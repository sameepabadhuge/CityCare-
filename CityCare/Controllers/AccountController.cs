using Microsoft.AspNetCore.Mvc;

namespace CityCare.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
