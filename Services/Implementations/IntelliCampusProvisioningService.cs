using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using NorviaBackoffice.Models;
using NorviaBackoffice.Models.Entities;
using NorviaBackoffice.Services.Interfaces;
namespace NorviaBackoffice.Services.Implementations;
public sealed class IntelliCampusProvisioningService(HttpClient client,IOptions<ProvisioningSettings> options):ITenantProvisioningService
{
    private sealed record ProvisionRequest(
        [property: JsonPropertyName("TenantID")] int TenantId,
        [property: JsonPropertyName("UserName")] string UserName,
        [property: JsonPropertyName("Email")] string Email,
        [property: JsonPropertyName("RoleID")] int RoleId);

    private sealed record ProvisionResponse(
        [property: JsonPropertyName("IsError")] bool IsError,
        [property: JsonPropertyName("Message")] string? Message,
        [property: JsonPropertyName("UserID")] string? UserId);

    public async Task<string> ProvisionAsync(UserExtended user,CancellationToken ct=default)
    {
        var settings=options.Value;
        if(string.IsNullOrWhiteSpace(settings.Endpoint)||string.IsNullOrWhiteSpace(settings.Key)||string.IsNullOrWhiteSpace(settings.Secret))
            throw new InvalidOperationException("Intelli Campus provisioning is not configured.");
        if(user.UserRoleId is null||string.IsNullOrWhiteSpace(user.Email))
            throw new InvalidOperationException("The tenant administrator requires an email address and role before provisioning.");

        var tenantId=checked((int)user.TenantId);
        var roleId=checked((int)user.UserRoleId.Value);
        var userName=string.Join(' ',new[]{user.Name,user.Surname}.Where(value=>!string.IsNullOrWhiteSpace(value)).Select(value=>value!.Trim()));
        if(string.IsNullOrWhiteSpace(userName))throw new InvalidOperationException("The tenant administrator requires a name before provisioning.");

        using var request=new HttpRequestMessage(HttpMethod.Post,settings.Endpoint);
        request.Headers.Add("key",settings.Key);
        request.Headers.Add("secret",settings.Secret);
        request.Content=JsonContent.Create(new ProvisionRequest(tenantId,userName,user.Email.Trim(),roleId));

        using var response=await client.SendAsync(request,ct);
        response.EnsureSuccessStatusCode();
        var result=await response.Content.ReadFromJsonAsync<ProvisionResponse>(cancellationToken:ct);
        if(result is null)throw new InvalidOperationException("Provisioning response was empty or invalid.");
        if(result.IsError)throw new InvalidOperationException($"Intelli Campus provisioning failed: {result.Message??"Unknown error."}");
        if(string.IsNullOrWhiteSpace(result.UserId))throw new InvalidOperationException("Provisioning response did not contain UserID.");
        if(!Guid.TryParse(result.UserId,out var userId))throw new InvalidOperationException("Provisioning response contained an invalid UserID.");
        return userId.ToString();
    }
}
