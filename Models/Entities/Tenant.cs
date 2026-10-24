namespace NorviaBackoffice.Models.Entities;
public sealed class Tenant { public long Id { get; set; } public string Name { get; set; } = ""; public bool IsActive { get; set; } public ICollection<UserExtended> Users { get; set; } = []; }
