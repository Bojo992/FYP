using Microsoft.AspNetCore.Http.HttpResults;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World! test 1");
app.MapGet("/test", (HttpContext context) =>
{
    var ipAddress = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();

    if (string.IsNullOrEmpty(ipAddress))
    {
        ipAddress = context.Connection.RemoteIpAddress?.ToString();
    }

    return Results.Ok(ipAddress);
});


app.Run();