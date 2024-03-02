using Microsoft.AspNetCore.Identity;
using pullow_api.Entities;

namespace pullow_api.Authentication
{
    public class ApplicationUser : IdentityUser
    {
        public List<ApplicationUserGoal> ApplicationUserGoals { get; set; }
        public List<Goal> Goals { get; set; }
    }
}
