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
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public UserController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> SearchUser(string name)
        {
            var userId = this.HttpContext?.User?.Claims?.FirstOrDefault(c => c.Type == "Id")?.Value ?? String.Empty;
            if (userId == null)
            {
                return Unauthorized();
            }


            var users = await _context.Users.Where(user => user.UserName.Contains(name)).Select(user => new { Id = user.Id, Name = user.UserName }).ToListAsync();

            return Ok(users);
        }
    }
}
