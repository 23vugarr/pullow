using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pullow_api;
using pullow_api.Authentication;
using pullow_api.Entities;
using System.ComponentModel.DataAnnotations;

namespace pullow_api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class GoalController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public GoalController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetGoals()
        {
            var userId = this.HttpContext?.User?.Claims?.FirstOrDefault(c => c.Type == "Id")?.Value ?? String.Empty;
            if (userId == null)
            {
                return Unauthorized();
            }


            var goals = await _context.UserGoals.Where(UserGoal =>  UserGoal.UserId == Guid.Parse(userId)).Select(UserGoal => new { 
            
                    Id = UserGoal.Goal.Id,
                    Title = UserGoal.Goal.Title
            }).ToListAsync();


            return Ok(goals);
        }


        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetGoal(Guid id)
        {
            var userId = this.HttpContext?.User?.Claims?.FirstOrDefault(c => c.Type == "Id")?.Value ?? String.Empty;
            if (userId == null)
            {
                return Unauthorized();
            }

            
            var userGoal = await _context.UserGoals.Include(UserGoal => UserGoal.Goal).ThenInclude(goal => goal.Users).Where(UserGoal => UserGoal.GoalId == id && UserGoal.UserId == Guid.Parse(userId)).FirstOrDefaultAsync();
            if(userGoal == null)
            {
                return NotFound();
            }

            var goal = await _context.Goals.Where(goal => goal.Id == id).Select(goal => new { Id = userGoal.Goal.Id, Title = userGoal.Goal.Title, Url = userGoal.Goal.Url, Duration = userGoal.Goal.CachedDuration, MeanPrice = userGoal.Goal.CachedMeanPrice, Users = goal.UserGoals.Select(userGoal => new {Id = userGoal.UserId, Name = userGoal.User.FullName, SavingAmount = userGoal.SavingAmount, MonthlyAmount = userGoal.MonthlyAmount}) }).FirstOrDefaultAsync();
            return Ok(goal);
        }


        [HttpPost]
        public async Task<IActionResult> CreateGoal(CreateGoalDto model)
        {

            var userId = this.HttpContext?.User?.Claims?.FirstOrDefault(c => c.Type == "Id")?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return Unauthorized();
            }

            var newGoal = new Goal { Id = Guid.NewGuid(), Title = model.Title, Url = model.Url, Users = new List<ApplicationUser>()};
            newGoal.Users.Add(user);

            await _context.Goals.AddAsync(newGoal);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("{id:guid}/users")]
        public async Task<IActionResult> AddUserToGoal(Guid id, [FromBody] AddUserToGoalDto model)
        {

            var userTokenId = this.HttpContext?.User?.Claims?.FirstOrDefault(c => c.Type == "Id")?.Value;
            if (userTokenId == null)
            {
                return Unauthorized();
            }

            var userGoal = await _context.UserGoals.Where(userGoal => userGoal.GoalId == id && userGoal.UserId == Guid.Parse(userTokenId)).FirstOrDefaultAsync();
            if (userGoal == null)
            {
                return NotFound();
            }

            var userToBeAdded = await _userManager.FindByIdAsync(model.UserId.ToString());

            if(userToBeAdded == null)
            {
                return NotFound();
            }

            var newUserGoal = new UserGoal { GoalId = id, User = userToBeAdded, MonthlyAmount = model.MonthlyAmount, SavingAmount = model.SavingAmount };

            await _context.UserGoals.AddAsync(newUserGoal);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }

    public class CreateGoalDto
    {
        public string Title { get; set; }
        public string Url { get; set; }

    }

    public class AddUserToGoalDto
    {
        public Guid UserId { get; set; }
        public int SavingAmount { get; set; }
        public int MonthlyAmount { get; set; }
    }
}
