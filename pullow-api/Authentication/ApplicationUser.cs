using Microsoft.AspNetCore.Identity;
using pullow_api.Entities;

namespace pullow_api.Authentication
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string FullName { get; set; }
        public int? GrossSalary { get; set; }
        public List<UserGoal> UserGoals { get; set; }
        public List<Goal> Goals { get; set; }
    }
}
