using GameLibrary.Data.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace GameLibrary.Web.Controllers;

//Public platform pages are read-only. All platform changes belong to Admin.
//The assignment does not require a login or authentication system.
public class PlatformsController : Controller
{
    private readonly IPlatformRepository platformRepository;

    public PlatformsController(IPlatformRepository platformRepository)
    {
        this.platformRepository = platformRepository;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var platforms = await platformRepository.GetAllAsync();
        return View(platforms);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var platform = await platformRepository.GetByIdAsync(id);
        if (platform is null) return NotFound();
        return View(platform);
    }
}
