using Microsoft.AspNetCore.Identity;

namespace JobFinder.Models
{
    public class Job
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Company { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Location { get; set; }
        public string URL { get; set; }
        public float SalaryMin { get; set; }
        public float SalaryMax { get; set; }
        public List<ApplicationUser> SavingUsers { get; set; } = new();
    }
}
