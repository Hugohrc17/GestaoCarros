using Microsoft.AspNetCore.Mvc;

namespace GestaoCarros.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Dashboard()
        {
            return View("~/Views/Home/Dashboard.cshtml");
        }
    }
}
