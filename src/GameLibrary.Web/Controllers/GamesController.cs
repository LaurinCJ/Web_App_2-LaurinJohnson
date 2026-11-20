using GameLibrary.Data.Models;
using GameLibrary.Data.Repositories;
using GameLibrary.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;

namespace GameLibrary.Web.Controllers;

public class GamesController : Controller
{
    private readonly IGameRepository gameRepository;
    private readonly IFranchiseRepository franchiseRepository;
    private readonly IPlatformRepository platformRepository;

    public GamesController(IGameRepository gameRepository,
        IFranchiseRepository franchiseRepository, IPlatformRepository platformRepository)
    {
        this.gameRepository = gameRepository;
        this.franchiseRepository = franchiseRepository;
        this.platformRepository = platformRepository;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? title, string? notes)
    {
        ViewData["TitleFilter"] = title;
        ViewData["NotesFilter"] = notes;
        var games = await gameRepository.SearchAsync(title, notes);
        return View(games);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var game = await gameRepository.GetByIdAsync(id);
        if (game is null) return NotFound();
        return View(game);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var model = new GameFormViewModel();
        await PopulateChoicesAsync(model);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(GameFormViewModel model)
    {
        await ValidateChoicesAsync(model);
        if (!ModelState.IsValid)
        {
            await PopulateChoicesAsync(model);
            return View(model);
        }

        //Explicit mapping limits saving to the fields this form edits.
        //Empty optional notes become an empty SQL string, not a SQL NULL.
        var game = new Game
        {
            Title = model.Title.Trim(),
            Notes = model.Notes?.Trim() ?? string.Empty,
            ReleaseYear = model.ReleaseYear,
            FranchiseId = model.FranchiseId,
            PlatformId = model.PlatformId
        };

        try
        {
            await gameRepository.InsertAsync(game);
            return RedirectToAction(nameof(Index));
        }
        catch (SqlException ex) when (ex.Number == 547)
        {
            //A related record may have been removed since validation.
            ModelState.AddModelError(string.Empty,
                "The selected franchise or platform is no longer available. Choose again.");
            await PopulateChoicesAsync(model);
            return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var game = await gameRepository.GetByIdAsync(id);
        if (game is null) return NotFound();

        var model = new GameFormViewModel
        {
            Id = game.Id,
            Title = game.Title,
            Notes = game.Notes,
            ReleaseYear = game.ReleaseYear,
            FranchiseId = game.FranchiseId,
            PlatformId = game.PlatformId
        };
        await PopulateChoicesAsync(model);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, GameFormViewModel model)
    {
        if (id != model.Id) return BadRequest();
        if (await gameRepository.GetByIdAsync(id) is null) return NotFound();

        await ValidateChoicesAsync(model);
        if (!ModelState.IsValid)
        {
            await PopulateChoicesAsync(model);
            return View(model);
        }

        var game = new Game
        {
            Id = id,
            Title = model.Title.Trim(),
            Notes = model.Notes?.Trim() ?? string.Empty,
            ReleaseYear = model.ReleaseYear,
            FranchiseId = model.FranchiseId,
            PlatformId = model.PlatformId
        };

        try
        {
            bool updated = await gameRepository.UpdateAsync(game);
            if (!updated) return NotFound();
            return RedirectToAction(nameof(Index));
        }
        catch (SqlException ex) when (ex.Number == 547)
        {
            ModelState.AddModelError(string.Empty,
                "The selected franchise or platform is no longer available. Choose again.");
            await PopulateChoicesAsync(model);
            return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var game = await gameRepository.GetByIdAsync(id);
        if (game is null) return NotFound();
        return View(game);
    }

    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        bool deleted = await gameRepository.DeleteAsync(id);
        if (!deleted) return NotFound();
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateChoicesAsync(GameFormViewModel model)
    {
        var franchises = await franchiseRepository.GetAllAsync();
        var platforms = await platformRepository.GetAllAsync();

        model.FranchiseChoices = franchises.Select(franchise => new SelectListItem
        {
            Value = franchise.Id.ToString(),
            Text = franchise.Name
        }).ToList();

        model.PlatformChoices = platforms.Select(platform => new SelectListItem
        {
            Value = platform.Id.ToString(),
            Text = platform.Name
        }).ToList();
    }

    private async Task ValidateChoicesAsync(GameFormViewModel model)
    {
        //A positive ID alone does not prove the related record exists.
        if (model.FranchiseId > 0 &&
            await franchiseRepository.GetByIdAsync(model.FranchiseId) is null)
            ModelState.AddModelError(nameof(model.FranchiseId), "Choose an existing franchise.");

        if (model.PlatformId > 0 &&
            await platformRepository.GetByIdAsync(model.PlatformId) is null)
            ModelState.AddModelError(nameof(model.PlatformId), "Choose an existing platform.");
    }
}
