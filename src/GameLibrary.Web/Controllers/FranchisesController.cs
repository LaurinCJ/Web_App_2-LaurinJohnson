using GameLibrary.Data.Models;
using GameLibrary.Data.Repositories;
using GameLibrary.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace GameLibrary.Web.Controllers;

public class FranchisesController : Controller
{
    private readonly IFranchiseRepository franchiseRepository;

    public FranchisesController(IFranchiseRepository franchiseRepository)
    {
        this.franchiseRepository = franchiseRepository;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var records = await franchiseRepository.GetAllAsync();
        return View(records);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var record = await franchiseRepository.GetByIdAsync(id);
        if (record is null) return NotFound();
        return View(record);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new FranchiseFormViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(FranchiseFormViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var record = new Franchise
        {
            Name = model.Name.Trim(),
            Developer = model.Developer.Trim()
        };

        try
        {
            await franchiseRepository.InsertAsync(record);
            return RedirectToAction(nameof(Index));
        }
        catch (SqlException ex) when (ex.Number is 2601 or 2627)
        {
            //The database enforces unique names, including concurrent saves.
            ModelState.AddModelError(nameof(model.Name), "A franchise with this name already exists.");
            return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var record = await franchiseRepository.GetByIdAsync(id);
        if (record is null) return NotFound();

        return View(new FranchiseFormViewModel
        {
            Id = record.Id,
            Name = record.Name,
            Developer = record.Developer
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, FranchiseFormViewModel model)
    {
        if (id != model.Id) return BadRequest();
        if (await franchiseRepository.GetByIdAsync(id) is null) return NotFound();
        if (!ModelState.IsValid) return View(model);

        var record = new Franchise
        {
            Id = id,
            Name = model.Name.Trim(),
            Developer = model.Developer.Trim()
        };

        try
        {
            bool updated = await franchiseRepository.UpdateAsync(record);
            if (!updated) return NotFound();
            return RedirectToAction(nameof(Index));
        }
        catch (SqlException ex) when (ex.Number is 2601 or 2627)
        {
            ModelState.AddModelError(nameof(model.Name), "A franchise with this name already exists.");
            return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var record = await franchiseRepository.GetByIdAsync(id);
        if (record is null) return NotFound();
        return View(record);
    }

    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var record = await franchiseRepository.GetByIdAsync(id);
        if (record is null) return NotFound();

        try
        {
            bool deleted = await franchiseRepository.DeleteAsync(id);
            if (!deleted) return NotFound();
            return RedirectToAction(nameof(Index));
        }
        catch (SqlException ex) when (ex.Number == 547)
        {
            //Preserve related games and explain why this delete was blocked.
            ModelState.AddModelError(string.Empty,
                "This franchise cannot be deleted while games reference it. Reassign or delete those games first.");
            return View("Delete", record);
        }
    }
}
