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


            var goals = await _context.ApplicationUserGoals.Where(applicationUserGoal =>  applicationUserGoal.ApplicationUserId == Guid.Parse(userId)).Select(applicationUserGoal => new { 
            
                    Id = applicationUserGoal.Goal.Id,
                    Title = applicationUserGoal.Goal.Title
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

            
            var applicationUserGoal = await _context.ApplicationUserGoals.Include(applicationUserGoal => applicationUserGoal.Goal).Where(applicationUserGoal => applicationUserGoal.GoalId == id && applicationUserGoal.ApplicationUserId == Guid.Parse(userId)).FirstOrDefaultAsync();


            return Ok(applicationUserGoal.Goal);
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

            var newGoal = new Goal { Id = Guid.NewGuid(), Title = model.Title, Url = model.Url, ApplicationUsers = new List<ApplicationUser>()};
            newGoal.ApplicationUsers.Add(user);

            await _context.Goals.AddAsync(newGoal);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }

    public class CreateGoalDto
    {
        public string Title { get; set; }
        public string Url { get; set; }

    }
}
