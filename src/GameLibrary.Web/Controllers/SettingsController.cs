using GameLibrary.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace GameLibrary.Web.Controllers;

public class SettingsController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        //Accept only known theme values, including when reading the cookie.
        var model = new SettingsViewModel
        {
            Theme = Request.Cookies["GameLibrary.Theme"] == "dark" ? "dark" : "light"
        };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Index(SettingsViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        //Remember this browser's preference across pages and visits.
        Response.Cookies.Append("GameLibrary.Theme", model.Theme, new CookieOptions
        {
            Expires = DateTimeOffset.UtcNow.AddYears(1),
            Path = "/",
            HttpOnly = true,
            SameSite = SameSiteMode.Lax,
            Secure = Request.IsHttps
        });

        //The redirected request includes the new cookie, so the layout can
        //immediately apply the theme without restarting the application.
        return RedirectToAction(nameof(Index));
    }
}
