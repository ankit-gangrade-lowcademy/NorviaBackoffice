namespace NorviaBackoffice.Models.Entities;
public sealed class NorviaRole { public int Id { get; set; } public string Name { get; set; } = ""; public string? Description { get; set; } public bool IsActive { get; set; } public ICollection<NorviaUser> Users { get; set; } = []; }
