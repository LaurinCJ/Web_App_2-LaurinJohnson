using System.ComponentModel.DataAnnotations;

namespace GameLibrary.Web.ViewModels;

public class PlatformFormViewModel
{
    //Zero is allowed when creating a record; SQL assigns the new ID.
    public int Id { get; set; }

    [Required]
    [StringLength(80)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(80)]
    public string Manufacturer { get; set; } = string.Empty;
}
