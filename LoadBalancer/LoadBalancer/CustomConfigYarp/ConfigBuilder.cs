using Yarp.ReverseProxy.Configuration;
using Yarp.ReverseProxy.Transforms;

namespace LoadBalancer;

public class ConfigBuilder
{
    private CustomConfig config;

    public ConfigBuilder()
    {
        config = new CustomConfig();
        
        AddDefaultFrontend();
    }

    public ConfigBuilder(IProxyConfig proxyConfig)
    {
        config = new CustomConfig();

        AddDefaultFrontend();
        
        config.EditableClusters.AddRange(proxyConfig.Clusters.ToList().Where(cluster => cluster.ClusterId != "internalFrontendCluster"));
        config.EditableRoutes.AddRange(proxyConfig.Routes.ToList().Where(route => route.RouteId != "internalFrontendRoute"));
    }

    public CustomConfig Build()
    {
        return config;
    }

    public void AddRoute(RouteConfig route)
    {
        config.EditableRoutes.Add(route);
    }

    public void AddCluster(ClusterConfig cluster)
    {
        config.EditableClusters.Add(cluster);
    }

    public void AddClusters(List<ClusterConfig> toList)
    {
        config.EditableClusters.AddRange(toList.Where(cluster => cluster.ClusterId != "internalFrontendCluster"));    
    }

    public void AddRoutes(List<RouteConfig> toList)
    {
        config.EditableRoutes.AddRange(toList.Where(route => route.RouteId != "internalFrontendRoute"));
    }



    private void AddDefaultFrontend()
    {
#if PRODUCTION
        var destinations = new Dictionary<string, DestinationConfig>();
        destinations.Add("default", new DestinationConfig() {Address = "https://bojo992.github.io/"});
        
        config.EditableClusters.Add(new ClusterConfig()
        {
            ClusterId = "internalFrontendCluster",
            Destinations = destinations.AsReadOnly()
        });
        
        config.EditableRoutes.Add(new RouteConfig()
            {
                RouteId = "internalFrontendRoute",
                ClusterId = "internalFrontendCluster",
                Match = new RouteMatch()
                {
                    Path = "/config/{**catch-all}"
                }
            }
            .WithTransformPathRemovePrefix(prefix: new PathString("/config"))
            .WithTransformPathSet(path: "/FYP-ui")
        );
#else
        var destinations = new Dictionary<string, DestinationConfig>();
        destinations.Add("default", new DestinationConfig() {Address = "http://localhost:3000"});
        
        Console.WriteLine("Dev route");
        
        config.EditableClusters.Add(new ClusterConfig()
        {
            ClusterId = "internalFrontendCluster",
            Destinations = destinations.AsReadOnly()
        });
        
        config.EditableRoutes.Add(new RouteConfig()
            {
                RouteId = "internalFrontendRoute",
                ClusterId = "internalFrontendCluster",
                Match = new RouteMatch()
                {
                    Path = "/config/{**catch-all}"
                }
            }
        );
#endif
    }
}