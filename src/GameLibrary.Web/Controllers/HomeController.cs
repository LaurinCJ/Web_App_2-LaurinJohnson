using Microsoft.AspNetCore.Mvc;

namespace GameLibrary.Web.Controllers;

public class HomeController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }
}
