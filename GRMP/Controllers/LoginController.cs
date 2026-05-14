using Microsoft.AspNetCore.Mvc;

namespace GRMP.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Login()
        {
            return View("LoginView");
        }
    }
}
