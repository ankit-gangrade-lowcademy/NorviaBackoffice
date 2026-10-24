using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
namespace NorviaBackoffice.Models.ViewModels;
public sealed class TenantFormViewModel { public long Id { get; set; } [Required, StringLength(255)] [Display(Name="Organization name")] public string Name { get; set; } = ""; [Display(Name="Active tenant")] public bool IsActive { get; set; } = true; }
public sealed class ProductFormViewModel { public int Id { get; set; } [Required, StringLength(255)] public string Name { get; set; } = ""; public string? Description { get; set; } }
public sealed class ProductRoleFormViewModel { public int Id { get; set; } [Required, Display(Name="Product")] public int ProductId { get; set; } [Required, Range(0,int.MaxValue), Display(Name="Product Role ID")] public int ProductRoleId { get; set; } [Required, StringLength(255), Display(Name="Role name")] public string Name { get; set; } = ""; [Required, Range(0,int.MaxValue)] public int Order { get; set; } public IEnumerable<SelectListItem> Products { get; set; } = []; }
public sealed class TenantAdminFormViewModel
{
    public long Id { get; set; }
    [Required, Display(Name="Tenant")] public long TenantId { get; set; }
    [Required, StringLength(150)] public string Name { get; set; } = "";
    [StringLength(150)] public string? Surname { get; set; }
    [Required, EmailAddress, StringLength(320)] public string Email { get; set; } = "";
    [Phone, StringLength(30), Display(Name="Phone number")] public string? PhoneNumber { get; set; }
    [StringLength(100), Display(Name="Internal ID")] public string? UserInternalId { get; set; }
    [Display(Name="Product role ID")] public long? UserRoleId { get; set; }
    [StringLength(1000)] public string? Address { get; set; }
    [Display(Name="Active user")] public bool IsActive { get; set; } = true;
    public string? OutSystemsUserId { get; set; }
    public DateTime? UpdatedOn { get; set; }
    public IEnumerable<SelectListItem> Tenants { get; set; } = [];
}
public sealed class BackofficeUserFormViewModel
{
    public long Id { get; set; }
    [Required, StringLength(150)] public string Name { get; set; } = "";
    [Required, EmailAddress, StringLength(320)] public string Email { get; set; } = "";
    [Required, Display(Name="Role")] public int RoleId { get; set; }
    [DataType(DataType.Password), MinLength(12)] public string? Password { get; set; }
    [Display(Name="Active account")] public bool IsActive { get; set; } = true;
    [Display(Name="Require password change")] public bool MustChangePassword { get; set; } = true;
    public IEnumerable<SelectListItem> Roles { get; set; } = [];
}
