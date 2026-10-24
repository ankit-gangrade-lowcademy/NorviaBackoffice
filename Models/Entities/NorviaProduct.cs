namespace NorviaBackoffice.Models.Entities;
public sealed class NorviaProduct { public int Id { get; set; } public string Name { get; set; } = ""; public string? Description { get; set; } public ICollection<ProductUserRole> Roles { get; set; } = []; }
