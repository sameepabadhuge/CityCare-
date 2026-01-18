using Microsoft.AspNetCore.Mvc;

namespace CityCare.Controllers
{
    public class StaffController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
