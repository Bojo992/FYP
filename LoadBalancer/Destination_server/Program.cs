using Destination_server.Controller;
using MySqlConnector;

var builder = WebApplication.CreateBuilder(args);
// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy
            // .WithOrigins("http://localhost:3000/")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowAnyOrigin();
        // .AllowCredentials();
    });
});

builder.Services.AddMySqlDataSource(builder.Configuration.GetConnectionString("Default")!);

// Add SignalR services
builder.Services.AddSignalR();
var app = builder.Build();
// Use CORS with the specified policy
app.UseCors("CorsPolicy");
// Use default files and static files
// app.UseDefaultFiles();
// app.UseStaticFiles();
// Map the MessagingHub to the "/hub" endpoint
app.MapHub<ChatController>("/hub");
// Run the application
app.Run();