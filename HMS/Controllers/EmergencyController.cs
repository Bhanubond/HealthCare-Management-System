using Microsoft.AspNetCore.Mvc;

namespace HMS.Controllers
{
    public class EmergencyController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
