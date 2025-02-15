using Microsoft.AspNetCore.Mvc;

namespace ChatRoomSystem.Controllers
{
    public class AuthController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }
        public IActionResult Signup()
        {
            return View();
        }
    }
}
