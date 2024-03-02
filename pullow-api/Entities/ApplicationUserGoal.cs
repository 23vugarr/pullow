using pullow_api.Authentication;

namespace pullow_api.Entities
{
    public class ApplicationUserGoal
    {
        public ApplicationUser ApplicationUser { get; set; }
        public Guid ApplicationUserId { get; set; }
        public Goal Goal { get; set; }
        public Guid GoalId { get; set; }

        public int SavingAmount { get; set; }
        public int MonthlyAmount { get; set; }

    }
}
