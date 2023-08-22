namespace JobFinder.Models.ViewModels
{
    public class JobsViewModel
    {
        public List<Job> Jobs { get; set; } = new();
        public string Keywords { get; set; }
        public string Location { get; set; }
        public string PageTitle { get; set; }
        public int PageNum { get; set; }
        public string SortBy { get; set; }
        public string SortDir { get; set; }
    }
}
