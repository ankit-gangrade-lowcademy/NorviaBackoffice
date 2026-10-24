namespace NorviaBackoffice.Models.Entities;
public sealed class ProductUserRole { public int Id { get; set; } public int ProductId { get; set; } public int ProductRoleId { get; set; } public string Name { get; set; } = ""; public int Order { get; set; } public NorviaProduct Product { get; set; } = null!; }
