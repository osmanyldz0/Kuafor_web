using Microsoft.AspNetCore.Mvc;

namespace berber4.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
