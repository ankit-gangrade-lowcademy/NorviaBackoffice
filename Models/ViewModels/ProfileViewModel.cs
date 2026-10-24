using System.ComponentModel.DataAnnotations;
namespace NorviaBackoffice.Models.ViewModels;
public sealed class ProfileViewModel { [Required, StringLength(150, MinimumLength = 2)] public string Name { get; set; } = ""; public string Email { get; set; } = ""; public string Role { get; set; } = ""; public bool IsActive { get; set; } public DateTime? LastLoginOn { get; set; } }
