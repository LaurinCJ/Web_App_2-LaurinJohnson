using System.ComponentModel.DataAnnotations;

namespace GameLibrary.Web.ViewModels;

public class FranchiseFormViewModel
{
    //Zero is allowed when creating a record; SQL assigns the new ID.
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(120)]
    public string Developer { get; set; } = string.Empty;
}
