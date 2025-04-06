using LoadBalancer;
using LoadBalancer.LBPolicy;
using LoadBalancer.Services;
using OpenTelemetry.Metrics;
using Yarp.ReverseProxy.Configuration;
using Yarp.ReverseProxy.LoadBalancing;

var builder = WebApplication.CreateBuilder(args);
var confBuilder = new ConfigBuilder();

#if PRODUCTION
    
#else
    confBuilder.AddCluster(new ClusterConfig()
    {
        ClusterId = "testcluster",
        LoadBalancingPolicy = "IPHashing",
        Destinations = new Dictionary<string, DestinationConfig>()
        {
            { "dest1", new DestinationConfig() { Address = "http://localhost:7100/" } },
            { "dest2", new DestinationConfig() { Address = "http://localhost:7200/" } },
        }
    });

    confBuilder.AddRoute(new RouteConfig()
    {
        RouteId = "testroute",
        ClusterId = "testcluster",
        Match = new RouteMatch()
        {
            Path = "/test/{**catch-all}"
        }
    });
#endif



CustomConfig customConfig = confBuilder.Build();

builder.Services.AddReverseProxy()
    .LoadFromMemory(customConfig.EditableRoutes, customConfig.EditableClusters);
builder.Services.AddScoped<IConfigService, ConfigService>();
builder.Services.AddSingleton<ILoadBalancingPolicy, TestLoadBalancingPolicy>();
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();

#if PRODUCTION
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy.WithOrigins("https://bojo992.github.io/FYP-ui/");
        policy.AllowAnyHeader();
        policy.AllowAnyMethod();
        policy.AllowAnyOrigin();
    });
});
#else
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:3000");
        policy.AllowAnyHeader();
        policy.AllowAnyMethod();
        policy.AllowCredentials();
    });
});
#endif

builder.Services.AddOpenTelemetry()
    .WithMetrics(builder =>
    {
        builder.AddPrometheusExporter();

        builder.AddMeter("Microsoft.AspNetCore.Hosting",
            "Microsoft.AspNetCore.Server.Kestrel");
        builder.AddView("http.server.request.duration",
            new ExplicitBucketHistogramConfiguration
            {
                Boundaries = new double[] { 0, 0.005, 0.01, 0.025, 0.05,
                    0.075, 0.1, 0.25, 0.5, 0.75, 1, 2.5, 5, 7.5, 10 }
            });
    });

var app = builder.Build();

app.MapPrometheusScrapingEndpoint();
app.MapReverseProxy();
app.MapControllers();
app.UseCors("CorsPolicy");

app.Run();
