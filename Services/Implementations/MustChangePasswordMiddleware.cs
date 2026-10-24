namespace NorviaBackoffice.Services.Implementations;
public sealed class MustChangePasswordMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        if (context.User.Identity?.IsAuthenticated == true && context.User.HasClaim("must_change_password", "true") && !context.Request.Path.StartsWithSegments("/Account/ChangePassword") && !context.Request.Path.StartsWithSegments("/Account/Logout") && !context.Request.Path.StartsWithSegments("/css") && !context.Request.Path.StartsWithSegments("/js") && !context.Request.Path.StartsWithSegments("/lib")) { context.Response.Redirect("/Account/ChangePassword"); return; }
        await next(context);
    }
}
