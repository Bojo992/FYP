namespace LoadBalancer.requests;

public class UpdateRouteRequest
{
    public string routeName { get; set; }
    public string routeMatch { get; set; }
    public string clusterId { get; set; }
}