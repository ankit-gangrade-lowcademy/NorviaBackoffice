using NorviaBackoffice.Models.Entities;
namespace NorviaBackoffice.Services.Interfaces;
public interface ITenantProvisioningService { Task<string> ProvisionAsync(UserExtended user,CancellationToken cancellationToken=default); }
