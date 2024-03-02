using pullow_api.Authentication;

namespace pullow_api.Entities
{
    public class Goal
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public int? CachedDuration { get; set; }
        public int? CachedMeanPrice { get; set; }
        public int? CachedExpectedPrice { get; set; }
        public List<UserGoal> UserGoals { get; set; }
        public List<ApplicationUser> Users { get; set; }
    }
}
