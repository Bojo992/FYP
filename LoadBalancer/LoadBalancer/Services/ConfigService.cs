using LoadBalancer.requests;
using LoadBalancer.Requests.Delete;
using Yarp.ReverseProxy.Configuration;

namespace LoadBalancer.Services;

public class ConfigService(IHttpContextAccessor httpContextAccessor) : IConfigService
{
    private readonly InMemoryConfigProvider _configProvider =
        httpContextAccessor.HttpContext?.RequestServices.GetRequiredService<InMemoryConfigProvider>()!;

    public IProxyConfig GetConfig()
    {
        return _configProvider.GetConfig();
    }

    public IProxyConfig AddCluster(AddClusterRequest request)
    {
        var configBuilder = new ConfigBuilder(GetConfig());

        var destinations = new Dictionary<string, DestinationConfig>();
        destinations.Add(request.directionName, new DestinationConfig() { Address = request.directionPath});

        configBuilder.AddCluster(new ClusterConfig()
            {
                ClusterId = request.clusterId,
                LoadBalancingPolicy = request.loadBalancingPolicy,
                Destinations = destinations.AsReadOnly()
            }
        );

        var conf = configBuilder.Build();

        _configProvider.Update(conf.EditableRoutes.AsReadOnly(), conf.EditableClusters.AsReadOnly());

        return conf;
    }

    public IProxyConfig AddRoute(AddRouteRequest request)
    {
        var configBuilder = new ConfigBuilder(GetConfig());

        configBuilder.AddRoute(new RouteConfig()
        {
            RouteId = request.routeId,
            ClusterId = request.clusterId,
            Match = new RouteMatch() { Path = request.path }
        });

        var conf = configBuilder.Build();

        _configProvider.Update(conf.EditableRoutes.AsReadOnly(), conf.EditableClusters.AsReadOnly());

        return conf;
    }

    public IProxyConfig AddClusterDirection(AddClusterDirectionRequest request)
    {
        var oldConfig = GetConfig();
        var configBuilder = new ConfigBuilder();

        configBuilder.AddRoutes(oldConfig.Routes.ToList());

        foreach (var cluster in oldConfig.Clusters.ToList().Where(cluster => cluster.ClusterId != "internalFrontendCluster"))
        {
            if (cluster.ClusterId == request.clusterId)
            {
                var updatedDirections = ToDictionary(cluster.Destinations!);
                updatedDirections.Add(request.directionName, new DestinationConfig() { Address = request.directionPath });
                
                configBuilder.AddCluster(new ClusterConfig()
                {
                    ClusterId = cluster.ClusterId,
                    LoadBalancingPolicy = cluster.LoadBalancingPolicy,
                    Destinations = updatedDirections
                });
            }
            else
            {
                configBuilder.AddCluster(cluster);
            }
        }

        var conf = configBuilder.Build();

        _configProvider.Update(conf.EditableRoutes.AsReadOnly(), conf.EditableClusters.AsReadOnly());

        return conf;
    }

    public IProxyConfig UpdateRoute(UpdateRouteRequest request)
    {
        var oldConfig = GetConfig();
        var configBuilder = new ConfigBuilder();

        configBuilder.AddClusters(oldConfig.Clusters.ToList());

        foreach (var route in oldConfig.Routes.ToList().Where(route => route.RouteId != "internalFrontendRoute"))
        {
            if (route.RouteId == request.routeName)
            {
                var updateRoute = new RouteConfig()
                {
                    RouteId = request.routeName,
                    ClusterId = request.clusterId,
                    Match = new RouteMatch() { Path = request.routeMatch }
                };

                configBuilder.AddRoute(updateRoute);
            }
            else
            {
                configBuilder.AddRoute(route);
            }
        }

        var conf = configBuilder.Build();

        _configProvider.Update(conf.EditableRoutes.AsReadOnly(), conf.EditableClusters.AsReadOnly());

        return conf;
    }

    public IProxyConfig UpdateClusterDirections(AddClusterDirectionRequest request)
    {
        var oldConfig = GetConfig();
        var configBuilder = new ConfigBuilder();

        configBuilder.AddRoutes(oldConfig.Routes.ToList());

        foreach (var cluster in oldConfig.Clusters.ToList().Where(cluster => cluster.ClusterId != "internalFrontendCluster"))
        {
            if (cluster.ClusterId == request.clusterId && cluster.Destinations != null &&
                cluster.Destinations.Count != 0)
            {
                var updatedDirections = ToDictionary(cluster.Destinations!);

                var updatedDirectionConfig = new DestinationConfig() { Address = request.directionPath };
                updatedDirections.Remove(request.originalDirectionName!);
                updatedDirections.Add(request.directionName, updatedDirectionConfig);

                var updatedCluster = new ClusterConfig()
                {
                    ClusterId = request.clusterId,
                    LoadBalancingPolicy = cluster.LoadBalancingPolicy,
                    Destinations = updatedDirections.AsReadOnly()
                };

                configBuilder.AddCluster(updatedCluster);
            }
            else
            {
                configBuilder.AddCluster(cluster);
            }
        }

        var conf = configBuilder.Build();

        _configProvider.Update(conf.EditableRoutes.AsReadOnly(), conf.EditableClusters.AsReadOnly());

        return conf;
    }

    public IProxyConfig UpdateClusterLoadBalancingPolicy(UpdateLoadBalancingPolicyRequest request)
    {
        var oldConfig = GetConfig();
        var configBuilder = new ConfigBuilder();

        configBuilder.AddRoutes(oldConfig.Routes.ToList());

        foreach (var cluster in oldConfig.Clusters.ToList().Where(cluster => cluster.ClusterId != "internalFrontendCluster"))
        {
            if (cluster.ClusterId == request.clusterId)
            {
                var updatedCluster = new ClusterConfig()
                {
                    ClusterId = request.clusterId,
                    LoadBalancingPolicy = request.loadBalancingPolicy,
                    Destinations = cluster.Destinations
                };

                configBuilder.AddCluster(updatedCluster);
            }
            else
            {
                configBuilder.AddCluster(cluster);
            }
        }

        var conf = configBuilder.Build();

        _configProvider.Update(conf.EditableRoutes.AsReadOnly(), conf.EditableClusters.AsReadOnly());

        return conf;
    }

    public IProxyConfig DeleteCluster(DeleteClusterRequest request)
    {
        var oldConfig = GetConfig();
        var configBuilder = new ConfigBuilder();

        configBuilder.AddRoutes(oldConfig.Routes.ToList());

        foreach (var cluster in oldConfig.Clusters.ToList().Where(cluster => cluster.ClusterId != request.clusterId && cluster.ClusterId != "internalFrontendCluster"))
        {
            configBuilder.AddCluster(cluster);
        }

        var conf = configBuilder.Build();

        _configProvider.Update(conf.EditableRoutes.AsReadOnly(), conf.EditableClusters.AsReadOnly());

        return conf;
    }

    public IProxyConfig DeleteClusterDirections(DeleteClusterDirectionRequest request)
    {
        var oldConfig = GetConfig();
        var configBuilder = new ConfigBuilder();

        configBuilder.AddRoutes(oldConfig.Routes.ToList());

        foreach (var cluster in oldConfig.Clusters.ToList().Where(cluster => cluster.ClusterId != "internalFrontendCluster"))
        {
            if (cluster.ClusterId == request.clusterId)
            {
                var newDestinations = cluster.Destinations!.Keys.Where(destination => destination != request.directionId).ToDictionary(destination => destination, destination => new DestinationConfig() { Address = cluster.Destinations[destination].Address });

                configBuilder.AddCluster(new ClusterConfig()
                {
                    ClusterId = cluster.ClusterId,
                    LoadBalancingPolicy = cluster.LoadBalancingPolicy,
                    Destinations = newDestinations
                });
            }
            else
            {
                configBuilder.AddCluster(cluster);
            }
        }

        var conf = configBuilder.Build();

        _configProvider.Update(conf.EditableRoutes.AsReadOnly(), conf.EditableClusters.AsReadOnly());

        return conf;
    }

    public IProxyConfig DeleteRoute(DeleteRouteRequest request)
    {
        var oldConfig = GetConfig();
        var configBuilder = new ConfigBuilder();

        configBuilder.AddClusters(oldConfig.Clusters.ToList());

        foreach (var route in oldConfig.Routes.ToList().Where(route => route.RouteId != request.routeId && route.RouteId != "internalFrontendRoute"))
        {
            configBuilder.AddRoute(route);
        }

        var conf = configBuilder.Build();

        _configProvider.Update(conf.EditableRoutes.AsReadOnly(), conf.EditableClusters.AsReadOnly());

        return conf;
    }

    private static Dictionary<string, DestinationConfig> ToDictionary(
        IReadOnlyDictionary<string, DestinationConfig> dict)
    {
        return dict.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    }
}