using CityCare.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CityCare.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        // Session values (if logged in)
        ViewBag.UserName = HttpContext.Session.GetString("UserName");
        ViewBag.UserRole = HttpContext.Session.GetString("UserRole");

        return View();
    }

    public IActionResult About()
    {
        return View();
    }

    // Default MVC error page
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
