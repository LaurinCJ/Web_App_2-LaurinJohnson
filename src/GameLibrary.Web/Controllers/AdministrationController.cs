using GameLibrary.Data.Models;
using GameLibrary.Data.Repositories;
using GameLibrary.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace GameLibrary.Web.Controllers;

public class AdministrationController : Controller
{
    private readonly IPlatformRepository platformRepository;

    public AdministrationController(IPlatformRepository platformRepository)
    {
        this.platformRepository = platformRepository;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var records = await platformRepository.GetAllAsync();
        return View(records);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new PlatformFormViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PlatformFormViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var record = new Platform
        {
            Name = model.Name.Trim(),
            Manufacturer = model.Manufacturer.Trim()
        };

        try
        {
            await platformRepository.InsertAsync(record);
            return RedirectToAction(nameof(Index));
        }
        catch (SqlException ex) when (ex.Number is 2601 or 2627)
        {
            //The database enforces unique names, including concurrent saves.
            ModelState.AddModelError(nameof(model.Name), "A platform with this name already exists.");
            return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var record = await platformRepository.GetByIdAsync(id);
        if (record is null) return NotFound();

        return View(new PlatformFormViewModel
        {
            Id = record.Id,
            Name = record.Name,
            Manufacturer = record.Manufacturer
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, PlatformFormViewModel model)
    {
        if (id != model.Id) return BadRequest();
        if (await platformRepository.GetByIdAsync(id) is null) return NotFound();
        if (!ModelState.IsValid) return View(model);

        var record = new Platform
        {
            Id = id,
            Name = model.Name.Trim(),
            Manufacturer = model.Manufacturer.Trim()
        };

        try
        {
            bool updated = await platformRepository.UpdateAsync(record);
            if (!updated) return NotFound();
            return RedirectToAction(nameof(Index));
        }
        catch (SqlException ex) when (ex.Number is 2601 or 2627)
        {
            ModelState.AddModelError(nameof(model.Name), "A platform with this name already exists.");
            return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var record = await platformRepository.GetByIdAsync(id);
        if (record is null) return NotFound();
        return View(record);
    }

    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var record = await platformRepository.GetByIdAsync(id);
        if (record is null) return NotFound();

        try
        {
            bool deleted = await platformRepository.DeleteAsync(id);
            if (!deleted) return NotFound();
            return RedirectToAction(nameof(Index));
        }
        catch (SqlException ex) when (ex.Number == 547)
        {
            //Preserve related games and explain why this delete was blocked.
            ModelState.AddModelError(string.Empty,
                "This platform cannot be deleted while games reference it. Reassign or delete those games first.");
            return View("Delete", record);
        }
    }
}
