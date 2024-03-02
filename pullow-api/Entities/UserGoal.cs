using pullow_api.Authentication;

namespace pullow_api.Entities
{
    public class UserGoal
    {
        public ApplicationUser User { get; set; }
        public Guid UserId { get; set; }
        public Goal Goal { get; set; }
        public Guid GoalId { get; set; }

        public int SavingAmount { get; set; }
        public int MonthlyAmount { get; set; }

    }
}
