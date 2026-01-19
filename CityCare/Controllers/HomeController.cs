using Microsoft.AspNetCore.Mvc;

namespace CityCare.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
