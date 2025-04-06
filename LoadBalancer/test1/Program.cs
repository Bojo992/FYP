var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// app.MapGet("/", () => "Hello World! test 1");
app.MapGet("/test", () => "Hello World! test 1");

app.Run();