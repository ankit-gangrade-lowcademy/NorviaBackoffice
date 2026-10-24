using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using NorviaBackoffice.Models;
using NorviaBackoffice.Models.Entities;
using NorviaBackoffice.Services.Implementations;

namespace NorviaBackoffice.Tests;

public sealed class ProvisioningServiceTests
{
    [Fact]
    public async Task ProvisionAsync_SendsExpectedContractAndReturnsUserId()
    {
        const string expectedUserId="d2719d01-0b2f-4d4a-80df-4d586bf5967c";
        var handler=new RecordingHandler(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content=new StringContent($$"""{"IsError":false,"Message":"Tenant administrator provisioned successfully.","UserID":"{{expectedUserId}}"}""",Encoding.UTF8,"application/json")
        });
        var service=CreateService(handler);
        var user=new UserExtended{Id=7,TenantId=101,Name="Ada",Surname="Lovelace",Email="ada@example.com",UserRoleId=2};

        var result=await service.ProvisionAsync(user);

        Assert.Equal(expectedUserId,result);
        Assert.Equal(HttpMethod.Post,handler.Request!.Method);
        Assert.Equal("https://labs.lowcademy.com/intelli-backoffice/ProvisionTenantAdmin.php",handler.Request.RequestUri!.ToString());
        Assert.Equal("test-key",handler.Request.Headers.GetValues("key").Single());
        Assert.Equal("test-secret",handler.Request.Headers.GetValues("secret").Single());
        using var body=JsonDocument.Parse(handler.Body!);
        Assert.Equal(101,body.RootElement.GetProperty("TenantID").GetInt32());
        Assert.Equal("Ada Lovelace",body.RootElement.GetProperty("UserName").GetString());
        Assert.Equal("ada@example.com",body.RootElement.GetProperty("Email").GetString());
        Assert.Equal(2,body.RootElement.GetProperty("RoleID").GetInt32());
    }

    [Fact]
    public async Task ProvisionAsync_RejectsApiErrorWithoutReturningAnId()
    {
        var handler=new RecordingHandler(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content=new StringContent("""{"IsError":true,"Message":"Rejected","UserID":null}""",Encoding.UTF8,"application/json")
        });
        var service=CreateService(handler);
        var user=new UserExtended{TenantId=101,Name="Ada",Email="ada@example.com",UserRoleId=2};

        var error=await Assert.ThrowsAsync<InvalidOperationException>(()=>service.ProvisionAsync(user));

        Assert.Contains("Rejected",error.Message);
    }

    private static IntelliCampusProvisioningService CreateService(HttpMessageHandler handler)=>new(
        new HttpClient(handler),
        Options.Create(new ProvisioningSettings
        {
            Endpoint="https://labs.lowcademy.com/intelli-backoffice/ProvisionTenantAdmin.php",
            Key="test-key",
            Secret="test-secret"
        }));

    private sealed class RecordingHandler(HttpResponseMessage response):HttpMessageHandler
    {
        public HttpRequestMessage? Request { get; private set; }
        public string? Body { get; private set; }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,CancellationToken cancellationToken)
        {
            Request=request;
            Body=request.Content is null?null:await request.Content.ReadAsStringAsync(cancellationToken);
            return response;
        }
    }
}
