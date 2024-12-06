using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using McqsDb.Models;

namespace Mcq.Controllers;

public class HomeController : BaseController
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        
            var roleId = HttpContext.Session.GetString("RoleId");  

            if (roleId == "3")
            {
                return RedirectToAction("Dashboard", "Student");
            }
            else if (roleId == "2")
            {
                return RedirectToAction("Dashboard", "Teacher");
            }

            return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
