using Microsoft.AspNetCore.Identity;

namespace JobFinder.Models
{
	public class ApplicationUser : IdentityUser
	{
		public List<Job> SavedJobs { get; set; } = new();
	}
}
