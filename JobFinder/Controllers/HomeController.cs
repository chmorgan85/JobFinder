using JobFinder.Data;
using JobFinder.Models;
using JobFinder.Models.JSON;
using JobFinder.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace JobFinder.Controllers
{
    public class HomeController : Controller
    {
		private readonly ApplicationDbContext context;

		public HomeController(ApplicationDbContext context)
        {
            this.context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Search(string keywords, string location, int page, string sortBy, string sortDir)
        {
            var jobsVM = new JobsViewModel()
            {
                Keywords = keywords,
                Location = location,
                PageTitle = "Search",
                PageNum = page,
                SortBy = sortBy,
                SortDir = sortDir
            };

            var client = new HttpClient();

			// Search Adzuna
			client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var url = $"https://api.adzuna.com/v1/api/jobs/us/search/{page}" +
                $"?app_id=dc9195f8" +
                $"&app_key=684d539a44ce6a24a8f29485f0c8cece" +
                $"&what={keywords}" +
                $"&where={location}";
            switch (sortBy)
            {
                case "date":
                    url += "&sort_by=date";
                    break;
                case "salary":
                    url += "&sort_by=salary";
                    break;
            }
            // Adzuna sort direction is broken, always results in 400: Bad Request

			var stream = await client.GetStreamAsync(url);
            var results = await JsonSerializer.DeserializeAsync<AdzunaResults>(stream);

            foreach (var result in results.results)
            {
                // Check if it already exists in the database
                var job = context.Job
                    .Include(j => j.SavingUsers)
                    // Adzuna redirect URL changes (e.g. not reliable), check everything else
                    .SingleOrDefault(j => 
                        j.Title == result.title &&
                        j.Description == result.description &&
                        j.Company == result.company.display_name &&
                        j.CreatedDate == result.created &&
                        j.Location == result.location.display_name &&
                        j.SalaryMin == result.salary_min &&
                        j.SalaryMax == result.salary_max);

                // If not, make a new one
                if (job == null)
                {
					job = new Job
					{
						Title = result.title,
						Description = result.description,
						Company = result.company.display_name,
						CreatedDate = result.created,
						Location = result.location.display_name,
						URL = result.redirect_url,
						SalaryMin = result.salary_min,
						SalaryMax = result.salary_max
					};
				}

                jobsVM.Jobs.Add(job);
            }



            // Search USAJOBS
            client.DefaultRequestHeaders.Add("Host", "data.usajobs.gov");
            client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "chmorgan85@gmail.com");
            client.DefaultRequestHeaders.Add("Authorization-Key", "Ow48hFKLKxfK6pXYE85Q8T/TzHib1jCbSimrYqNerek=");

            url = $"https://data.usajobs.gov/api/search" +
                $"?Keyword={keywords}" +
                $"&LocationName={location}" +
                $"&Page={page}";
            if (sortBy == "date" || sortBy == "salary")
            {
                url += "&SortField=";
                if (sortBy == "date")
                {
                    url += "opendate";
                }
                else if (sortBy == "salary")
                {
                    url += "salary";
                }

                url += $"&SortDirection={sortDir}";
            }

			stream = await client.GetStreamAsync(url);
            var USAJOBSResult = await JsonSerializer.DeserializeAsync<USAJOBSResult>(stream);

            foreach (var result in USAJOBSResult.SearchResult.SearchResultItems)
            {
				// Check if it already exists in the database
				var job = context.Job
                    .Include(j => j.SavingUsers)
                    .SingleOrDefault(j => j.URL == result.MatchedObjectDescriptor.PositionURI);

				// If not, make a new one
				if (job == null)
                {
					job = new Job
					{
						Title = result.MatchedObjectDescriptor.PositionTitle,
						Description = result.MatchedObjectDescriptor.QualificationSummary,
						Company = result.MatchedObjectDescriptor.OrganizationName,
						CreatedDate = result.MatchedObjectDescriptor.PublicationStartDate,
						Location = result.MatchedObjectDescriptor.PositionLocationDisplay,
						URL = result.MatchedObjectDescriptor.PositionURI,
						SalaryMin = float.Parse(result.MatchedObjectDescriptor.PositionRemuneration[0].MinimumRange),
						SalaryMax = float.Parse(result.MatchedObjectDescriptor.PositionRemuneration[0].MaximumRange)
					};
				}

                jobsVM.Jobs.Add(job);
            }



            // Search Findwork
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Accept.Clear();
			client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("Authorization", "Token a179bb452ab605f6f34b55906e06846c76995683");

            url = $"https://findwork.dev/api/jobs/" +
                $"?page={page}" +
                $"&search={keywords}";
            // Findwork has very limited support for location (most results report "null" for location)
            switch (sortBy)
            {
                case "date":
                    url += "&order_by=date";
                    break;
				case "relevance":
					url += "&order_by=relevance";
					break;
                // Findwork doesn't support salary information
			}

			stream = await client.GetStreamAsync(url);
            var findworkRoot = await JsonSerializer.DeserializeAsync<FindworkRoot>(stream);

            foreach (var result in findworkRoot.results)
            {
				// Check if it already exists in the database
				var job = context.Job
					.Include(j => j.SavingUsers)
					.SingleOrDefault(j => j.URL == result.url);

				// If not, make a new one
				if (job == null)
				{
					job = new Job
					{
						Title = result.role,
						Description = Regex.Replace(result.text, "<.*?>", String.Empty),
						Company = result.company_name,
						CreatedDate = result.date_posted,
						Location = result.location,
						URL = result.url
					};
				}

				jobsVM.Jobs.Add(job);
			}



			// Sort results
			switch (sortBy)
            {
                case "date":
                    switch (sortDir)
                    {
                        case "desc":
							jobsVM.Jobs.Sort((x, y) => y.CreatedDate.CompareTo(x.CreatedDate));
							break;
                        case "asc":
							jobsVM.Jobs.Sort((x, y) => x.CreatedDate.CompareTo(y.CreatedDate));
							break;
                    }
                    break;

                case "salary":
					switch (sortDir)
					{
						case "desc":
							jobsVM.Jobs.Sort((x, y) => y.SalaryMin.CompareTo(x.SalaryMin));
							break;
						case "asc":
							jobsVM.Jobs.Sort((x, y) => x.SalaryMin.CompareTo(y.SalaryMin));
							break;
					}
					break;
            }

            return View("Jobs", jobsVM);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}