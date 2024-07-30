using Microsoft.AspNetCore.Mvc;

namespace Azure.Storge.Controllers
{
    public class AttendeeRegistrationController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
