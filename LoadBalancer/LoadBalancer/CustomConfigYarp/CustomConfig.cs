using Microsoft.Extensions.Primitives;
using Yarp.ReverseProxy.Configuration;

namespace LoadBalancer;

public class CustomConfig : IProxyConfig
{
    public List<ClusterConfig> EditableClusters { get; } = new List<ClusterConfig>();
    public List<RouteConfig> EditableRoutes { get; } = new List<RouteConfig>();
    
    public IReadOnlyList<RouteConfig> Routes { get; }
    public IReadOnlyList<ClusterConfig> Clusters { get; }
    public IChangeToken ChangeToken { get; }
}