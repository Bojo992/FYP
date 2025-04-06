using System.Net.Http.Headers;
using System.Security.Cryptography.Xml;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/test", () => "Hello World! test 3");
app.MapGet("/new-endpoint", () => "Hello World! test 3");

app.Run();