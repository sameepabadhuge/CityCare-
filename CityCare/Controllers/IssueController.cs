using Microsoft.AspNetCore.Mvc;

namespace CityCare.Controllers
{
    public class IssueController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
