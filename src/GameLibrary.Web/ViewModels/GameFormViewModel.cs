using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GameLibrary.Web.ViewModels;

public class GameFormViewModel
{
    //A new game starts at zero; an edit preserves the database ID.
    public int Id { get; set; }

    [Required]
    [StringLength(120)]
    public string Title { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Notes { get; set; }

    [Display(Name = "Release year")]
    [Range(1970, 2100)]
    public int ReleaseYear { get; set; } = DateTime.Today.Year;

    [Display(Name = "Franchise")]
    [Range(1, int.MaxValue, ErrorMessage = "Choose a franchise.")]
    public int FranchiseId { get; set; }

    [Display(Name = "Platform")]
    [Range(1, int.MaxValue, ErrorMessage = "Choose a platform.")]
    public int PlatformId { get; set; }

    //These lists are supplied by the controller, not submitted as form data.
    [ValidateNever]
    public List<SelectListItem> FranchiseChoices { get; set; } = new();

    [ValidateNever]
    public List<SelectListItem> PlatformChoices { get; set; } = new();
}
