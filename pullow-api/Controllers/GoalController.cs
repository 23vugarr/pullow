using HtmlAgilityPack;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pullow_api;
using pullow_api.Authentication;
using pullow_api.Entities;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml;

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

            Uri uri = new Uri(model.Url);
            var host = uri.Host switch
            {
                "www.turbo.az" => "turbo",
                "turbo.az" => "turbo",
                "www.bina.az" => "bina",
                "bina.az" => "bina",
                _ => throw new ArgumentException("Please enter correct url type!")
            };

            int mean = 0;

            using (WebClient client = new WebClient())
            {
                string htmlCode = client.DownloadString(model.Url);
                HtmlDocument doc = new HtmlDocument();

                doc.LoadHtml(htmlCode);

                
                if(host == "turbo")
                {
                    var productList = doc.DocumentNode.SelectSingleNode("//p[normalize-space(text())='ELANLAR']/../..").NextSibling.LastChild;
                    
                    var sum = 0;
                    var count = 0;
                    foreach(var product in productList.ChildNodes)
                    {
                        var isDollar = false;
                        var priceStr = product.SelectSingleNode(".//div[contains(concat(' ', @class, ' '),'product-price')]")?.InnerText;
                        if (priceStr == null)
                        {
                            priceStr = product.SelectSingleNode(".//div[contains(concat(' ', @class, ' '),'product-price') and not(span)]")?.InnerText;
                        }
                        if (priceStr.Contains("$"))
                        {
                            isDollar = true;
                        }
                        string cleanedInput = Regex.Replace(priceStr, "[^0-9 ]", "");
                        cleanedInput = cleanedInput.Replace(" ", "");


                        if (Int32.TryParse(cleanedInput, out int realPrice))
                        {
                            realPrice = isDollar ? Convert.ToInt32(realPrice * 1.7) : realPrice;

                            sum += realPrice;
                            count += 1;
                        }
                    }
                    mean = Convert.ToInt32(sum / count);
                }

                else if (host == "bina")
                {
                    var productList = doc.DocumentNode.SelectSingleNode("//div[normalize-space(text())='ELANLAR']/..")?.NextSibling;

                    var sum = 0;
                    var count = 0;
                    foreach (var product in productList.ChildNodes)
                    {
                        var isDollar = false;

                        var priceVal = product.SelectSingleNode(".//span[contains(concat(' ', @class, ' '),'price-val')]")?.InnerText ?? "";
                        var priceCur = product.SelectSingleNode(".//span[contains(concat(' ', @class, ' '),'price-cur')]")?.InnerText ?? "";

                        if (priceCur == "USD" || priceCur == "$")
                        {
                            isDollar = true;
                        }

                        string cleanedInput = Regex.Replace(priceVal, "[^0-9 ]", "");
                        cleanedInput = cleanedInput.Replace(" ", "");

                        if (Int32.TryParse(cleanedInput, out int realPrice))
                        {
                            realPrice = isDollar ? Convert.ToInt32(realPrice * 1.7) : realPrice;

                            sum += realPrice;
                            count += 1;
                        }
                    }
                    mean = Convert.ToInt32(sum / count);
                }

            }

            var newGoal = new Goal { Id = Guid.NewGuid(), Title = model.Title, Url = model.Url, Users = new List<ApplicationUser>(), CachedMeanPrice = mean};
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

        [HttpPut("{id:guid}/users/{userId:guid}")]
        public async Task<IActionResult> ChangeUserParams(Guid id,Guid userId, [FromBody] ChangeUserParamsDto model)
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

            var userGoalToBeChanged = await _context.UserGoals.Where(userGoal => userGoal.GoalId == id && userGoal.UserId == userId).FirstOrDefaultAsync();

            userGoalToBeChanged.MonthlyAmount = model.MonthlyAmount;
            userGoalToBeChanged.SavingAmount = model.SavingAmount;

            await _context.SaveChangesAsync();

            return Ok();
        }


        [HttpPost("{id:guid}/strategy")]
        public async Task<IActionResult> UpsertGoalStrategy(Guid id, [FromBody] AddUserToGoalDto model)
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


            await _context.SaveChangesAsync();

            return Ok();
        }
    }

    public class CreateGoalDto
    {
        public string Title { get; set; }
        public string Url { get; set; }

    }


    public class ChangeUserParamsDto
    {
        public int SavingAmount { get; set; }
        public int MonthlyAmount { get; set; }
    }

    public class AddUserToGoalDto
    {
        public Guid UserId { get; set; }
        public int SavingAmount { get; set; }
        public int MonthlyAmount { get; set; }
    }
}
