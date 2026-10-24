using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using NorviaBackoffice.Data;
using NorviaBackoffice.Models;
using NorviaBackoffice.Models.Entities;
using NorviaBackoffice.Services.Implementations;
using NorviaBackoffice.Services.Interfaces;

namespace NorviaBackoffice;

public partial class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connectionString) && !builder.Environment.IsEnvironment("Testing"))
            throw new InvalidOperationException("ConnectionStrings:DefaultConnection is required. Configure it with User Secrets or environment variables.");

        builder.Services.AddControllersWithViews();
        builder.Services.AddDbContext<NorviaDbContext>(options => options.UseSqlServer(connectionString ?? "Server=(local);Database=Norvia;"));
        builder.Services.AddDataProtection();
        builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection(EmailSettings.SectionName));
        builder.Services.Configure<ApplicationSettings>(builder.Configuration.GetSection(ApplicationSettings.SectionName));
        builder.Services.Configure<LockoutSettings>(builder.Configuration.GetSection(LockoutSettings.SectionName));
        builder.Services.Configure<ProvisioningSettings>(builder.Configuration.GetSection(ProvisioningSettings.SectionName));
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<IEmailSender, SmtpEmailSender>();
        builder.Services.AddSingleton<IPasswordResetTokenStore, ProtectedFilePasswordResetTokenStore>();
        builder.Services.AddSingleton<ILoginThrottle, MemoryLoginThrottle>();
        builder.Services.AddHttpClient<ITenantProvisioningService, IntelliCampusProvisioningService>((services,client) => { var settings=services.GetRequiredService<Microsoft.Extensions.Options.IOptions<ProvisioningSettings>>().Value; client.Timeout=TimeSpan.FromSeconds(settings.TimeoutSeconds); });
        builder.Services.AddSingleton<Microsoft.AspNetCore.Identity.IPasswordHasher<NorviaUser>, Microsoft.AspNetCore.Identity.PasswordHasher<NorviaUser>>();
        builder.Services.AddScoped<NorviaCookieAuthenticationEvents>();

        builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
        {
            options.LoginPath = "/Account/Login";
            options.AccessDeniedPath = "/Account/AccessDenied";
            options.Cookie.Name = "Norvia.Backoffice.Auth";
            options.Cookie.HttpOnly = true;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            options.Cookie.SameSite = SameSiteMode.Lax;
            options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
            options.SlidingExpiration = true;
            options.EventsType = typeof(NorviaCookieAuthenticationEvents);
        });
        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("SuperAdmin", p => p.RequireRole("Super Admin"));
            options.AddPolicy("Administrator", p => p.RequireRole("Super Admin", "Admin"));
        });

        var app = builder.Build();
        if (!app.Environment.IsDevelopment()) { app.UseExceptionHandler("/Home/Error"); app.UseHsts(); }
        app.UseForwardedHeaders(new ForwardedHeadersOptions { ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto });
        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthentication();
        app.UseMiddleware<MustChangePasswordMiddleware>();
        app.UseAuthorization();
        app.MapStaticAssets();
        app.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}").WithStaticAssets();
        app.Run();
    }
}

