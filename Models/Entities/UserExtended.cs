namespace NorviaBackoffice.Models.Entities;
public sealed class UserExtended
{
    public long Id { get; set; }
    public string? OutSystemsUserId { get; set; }
    public long TenantId { get; set; }
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public string? UserInternalId { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public long? UserRoleId { get; set; }
    public string? Address { get; set; }
    public bool IsStudent { get; set; }
    public string? EnrolledCourse { get; set; }
    public int? CurrentYear { get; set; }
    public long? CreatedBy { get; set; }
    public DateTime? CreatedOn { get; set; }
    public long? UpdatedBy { get; set; }
    public DateTime? UpdatedOn { get; set; }
    public bool IsActive { get; set; }
    public Tenant Tenant { get; set; } = null!;
}
