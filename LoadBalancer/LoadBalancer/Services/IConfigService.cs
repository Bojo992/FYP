using LoadBalancer.requests;
using LoadBalancer.Requests.Delete;
using Yarp.ReverseProxy.Configuration;

namespace LoadBalancer.Services;

public interface IConfigService
{
    public IProxyConfig GetConfig();
    
    public IProxyConfig AddCluster(AddClusterRequest request);
    public IProxyConfig AddRoute(AddRouteRequest request);
    public IProxyConfig AddClusterDirection(AddClusterDirectionRequest request);
    
    public IProxyConfig UpdateRoute(UpdateRouteRequest route);
    public IProxyConfig UpdateClusterDirections(AddClusterDirectionRequest direction);
    public IProxyConfig UpdateClusterLoadBalancingPolicy(UpdateLoadBalancingPolicyRequest policy);

    public IProxyConfig DeleteCluster(DeleteClusterRequest cluster);
    public IProxyConfig DeleteClusterDirections(DeleteClusterDirectionRequest cluster);
    public IProxyConfig DeleteRoute(DeleteRouteRequest routeId);
}