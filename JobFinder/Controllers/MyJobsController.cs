using JobFinder.Data;
using JobFinder.Models;
using JobFinder.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JobFinder.Controllers
{
	[Authorize]
	public class MyJobsController : Controller
	{
		private readonly ApplicationDbContext context;
		private UserManager<ApplicationUser> userManager;

		public MyJobsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
		{
			this.context = context;
			this.userManager = userManager;
		}

		public async Task<IActionResult> Index()
		{
			// Get all jobs saved by the current user
			var thisUser = await userManager.GetUserAsync(User);
			var jobs = context.Users
				.Include(u => u.SavedJobs)
				.Single(u => u == thisUser)
				.SavedJobs;

			return View("Jobs", new JobsViewModel
			{
				Jobs = jobs,
				Keywords = "",
				Location = "",
				PageTitle = "My Jobs"
			});
		}

		public async Task<IActionResult> Save(string title, string url, string company, string location, 
			float salaryMin, float salaryMax, string description, DateTime createdDate)
		{
			// Check if it's already in the database
			var job = context.Job
				.Include(j => j.SavingUsers)
				// URL is not reliable (thanks to Adzuna), check everything else
				.SingleOrDefault(j =>
					j.Title == title &&
					j.Description == description &&
					j.Company == company &&
					j.CreatedDate == createdDate &&
					j.Location == location &&
					j.SalaryMin == salaryMin &&
					j.SalaryMax == salaryMax);

			// If not, make a new one
			if (job == null)
			{
				job = new Job
				{
					Title = title,
					URL = url,
					Company = company,
					Location = location,
					SalaryMin = salaryMin,
					SalaryMax = salaryMax,
					Description = description,
					CreatedDate = createdDate,
				};
				job.SavingUsers.Add(await userManager.GetUserAsync(User));
				context.Job.Add(job);
			}
			else
			{
				job.SavingUsers.Add(await userManager.GetUserAsync(User));
			}

			context.SaveChanges();

			return Ok();
		}

		public async Task<IActionResult> Unsave(string title, string url, string company, string location,
			float salaryMin, float salaryMax, string description, DateTime createdDate)
		{
			var job = context.Job
				.Include(j => j.SavingUsers)
				// URL is not reliable (thanks to Adzuna), check everything else
				.SingleOrDefault(j =>
					j.Title == title &&
					j.Description == description &&
					j.Company == company &&
					j.CreatedDate == createdDate &&
					j.Location == location &&
					j.SalaryMin == salaryMin &&
					j.SalaryMax == salaryMax);

			if (job == null)
			{
				return NotFound();
			}

			// If the current user isn't saving this job, something went wrong.
			var userIsRemoved = job.SavingUsers.Remove(await userManager.GetUserAsync(User));
			if (!userIsRemoved)
			{
				return BadRequest();
			}

			// If no one else is saving this job then remove it from the database
			if (job.SavingUsers.Count == 0)
			{
				context.Remove(job);
			}

			context.SaveChanges();

			return Ok();
		}
	}
}
