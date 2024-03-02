using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Linq;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenLocalhost(8000); // Listen on localhost:8000
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/error");
}

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();

[ApiController]
public class ApiController : ControllerBase
{
    private readonly IHostApplicationLifetime _appLifetime;

    public ApiController(IHostApplicationLifetime appLifetime)
    {
        _appLifetime = appLifetime;
    }

    [HttpGet("get_key")]
    public ActionResult<string> GetKey()
    {
        foreach (var drive in DriveInfo.GetDrives().Where(d => d.DriveType == DriveType.Removable))
        {
            string keyFilePath = Path.Combine(drive.Name, "key");
            if (System.IO.File.Exists(keyFilePath))
            {
                return System.IO.File.ReadAllText(keyFilePath);
            }
        }

        return NotFound("Key file not found on any USB drive.");
    }

    [HttpPost("shutdown")]
    public IActionResult Shutdown()
    {
        // Implement appropriate security checks here

        _appLifetime.StopApplication();
        return Ok("Application is shutting down...");
    }
}
