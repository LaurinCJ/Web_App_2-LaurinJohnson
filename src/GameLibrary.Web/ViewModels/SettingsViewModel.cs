using System.ComponentModel.DataAnnotations;

namespace GameLibrary.Web.ViewModels;

public class SettingsViewModel
{
    [Required]
    [RegularExpression("^(light|dark)$", ErrorMessage = "Choose Light or Dark.")]
    public string Theme { get; set; } = "light";
}
