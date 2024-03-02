using HtmlAgilityPack;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using pullow_api;
using pullow_api.Authentication;
using pullow_api.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text;
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
        public async Task<IActionResult> UpsertGoalStrategy(Guid id)
        {

            var userTokenId = this.HttpContext?.User?.Claims?.FirstOrDefault(c => c.Type == "Id")?.Value;
            if (userTokenId == null)
            {
                return Unauthorized();
            }

            var userGoal = await _context.UserGoals.Include(userGoal => userGoal.Goal).Where(userGoal => userGoal.GoalId == id && userGoal.UserId == Guid.Parse(userTokenId)).FirstOrDefaultAsync();
            if (userGoal == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(userTokenId);

            double superGross = 0;

            using (HttpClient client = new HttpClient())
            {
                // URL to send the POST request
                string url = "https://mylife.az/online-calculator/salary-converter-api/insurance-fee/net-to-gross";

                // Data to be sent in the request body
                string postData = $"sector=&customerId=1938&grossSalary={user.GrossSalary}&netInsuranceFee={userGoal.MonthlyAmount}&currency=AZN&netSalary=10";

                // Set request headers
                client.DefaultRequestHeaders.Add("Sec-Ch-Ua", "\"Chromium\";v=\"121\", \"Not A(Brand\";v=\"99\"");
                client.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");
                client.DefaultRequestHeaders.Add("Sec-Ch-Ua-Mobile", "?0");
                client.DefaultRequestHeaders.Add("Sec-Ch-Ua-Platform", "\"Windows\"");
                client.DefaultRequestHeaders.Add("Origin", "https://mylife.az");
                client.DefaultRequestHeaders.Add("Sec-Fetch-Site", "same-origin");
                client.DefaultRequestHeaders.Add("Sec-Fetch-Mode", "cors");
                client.DefaultRequestHeaders.Add("Sec-Fetch-Dest", "empty");
                client.DefaultRequestHeaders.Add("Referer", "https://mylife.az/onlineapp");
                client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
                client.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.9");
                client.DefaultRequestHeaders.Add("Priority", "u=1, i");

                // Create the HTTP content
                HttpContent content = new StringContent(postData, Encoding.UTF8, "application/x-www-form-urlencoded");

                // Send the POST request
                HttpResponseMessage response = await client.PostAsync(url, content);

                // Check if the response is successful
                if (response.IsSuccessStatusCode)
                {
                    // Read the response content
                    string responseContent = await response.Content.ReadAsStringAsync();
                    dynamic jsonResponse = JsonConvert.DeserializeObject(responseContent);
                    superGross = jsonResponse.data.superGrossInsuranceFee;

                }
                else
                {
                    return BadRequest();
                }
            }

            Dictionary<string, string> latinToAzeri = new Dictionary<string, string>
            {
                {"nizami", "Nizami" },
                { "yasamal", "Yasamal"},
                { "sebail", "Səbail" },
                { "xetai", "Xətai" },
                { "sabuncu", "Sabunçu" },
                { "nesimi", "Nəsimi" },
                { "qaradag", "Qaradağ" },
                { "bineqedi", "Binəqədi" },
                { "nerimanov", "Nərimanov" }
            };

            var regionForApi = FindMatch(userGoal.Goal.Url, latinToAzeri);

            if(regionForApi != null)
            {

                string apiUrl = "http://3.223.46.152/model/";
                string jsonBody = $"{{\"city\": \"{regionForApi}\", \"price\": {userGoal.Goal.CachedMeanPrice}}}";

                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("API_key", "test");

                    StringContent content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync(apiUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        var responseObject = JsonConvert.DeserializeObject<ResponseObject>(responseBody);

                        // Check if data is not null and contains result
                        if (responseObject?.Data?.Result != null)
                        {
                            // Extract the result dictionary
                            Dictionary<string, int> resultDictionary = responseObject.Data.Result;

                            int? selectedMonth = null;


                            using (HttpClient client2 = new HttpClient())
                            {
                                foreach(var key in resultDictionary.Keys)
                                {

                                }
                                foreach(var key in resultDictionary.Keys)
                                {
                                    // URL to send the POST request
                                    string url = "https://mylife.az/online-calculator/salary-converter-api/insurance-fee/insurance-amount";

                                    // Data to be sent in the request body
                                    string postData = $"customerId=1938&sector=&birthdate=06.02.2003&insuranceFeePaymentType=12&superGrossInsuranceFee={superGross}&netInsuranceFee=1&contractPeriod={key}&currency=AZN&calculatorCreditInput=";

                                    // Set request headers
                                    client2.DefaultRequestHeaders.Add("Sec-Ch-Ua", "\"Chromium\";v=\"121\", \"Not A(Brand\";v=\"99\"");
                                    client2.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");
                                    client2.DefaultRequestHeaders.Add("Sec-Ch-Ua-Mobile", "?0");
                                    client2.DefaultRequestHeaders.Add("Sec-Ch-Ua-Platform", "\"Windows\"");
                                    client2.DefaultRequestHeaders.Add("Origin", "https://mylife.az");
                                    client2.DefaultRequestHeaders.Add("Sec-Fetch-Site", "same-origin");
                                    client2.DefaultRequestHeaders.Add("Sec-Fetch-Mode", "cors");
                                    client2.DefaultRequestHeaders.Add("Sec-Fetch-Dest", "empty");
                                    client2.DefaultRequestHeaders.Add("Referer", "https://mylife.az/onlineapp");
                                    client2.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
                                    client2.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.9");
                                    client2.DefaultRequestHeaders.Add("Priority", "u=1, i");

                                    // Create the HTTP content
                                    HttpContent content2 = new StringContent(postData, Encoding.UTF8, "application/x-www-form-urlencoded");

                                    // Send the POST request
                                    HttpResponseMessage response2 = await client2.PostAsync(url, content2);

                                    // Check if the response is successful
                                    if (response2.IsSuccessStatusCode)
                                    {
                                        // Read the response content
                                        string responseContent = await response2.Content.ReadAsStringAsync();
                                        dynamic jsonResponse = JsonConvert.DeserializeObject(responseContent);
                                        double hysInvestment = jsonResponse.data.hysInvestment;

                                        if(hysInvestment > resultDictionary[key])
                                        {
                                            selectedMonth = Convert.ToInt32(key);
                                            userGoal.Goal.CachedDuration = selectedMonth;
                                            userGoal.Goal.CachedExpectedPrice = resultDictionary[key];

                                        }

                                    }
                                    else
                                    {
                                        return BadRequest();
                                    }
                                }
                                
                            }

                        }
                        else
                        {
                            Console.WriteLine("No result found in the response.");
                        }
                    }
                }



            }

            await _context.SaveChangesAsync();

            return Ok();
        }

        static string FindMatch(string url, Dictionary<string, string> latinToAzeri)
        {
            foreach (var key in latinToAzeri.Keys)
            {
                if (url.Contains(key))
                {
                    return latinToAzeri[key];
                }
            }
            return null;
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

    public class ResponseObject
    {
        public DataObject Data { get; set; }
        public object Detail { get; set; }
    }

    public class DataObject
    {
        public Dictionary<string, int> Result { get; set; }
    }
}
