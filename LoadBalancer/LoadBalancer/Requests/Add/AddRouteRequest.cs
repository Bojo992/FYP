namespace LoadBalancer.requests;

public class AddRouteRequest
{
    public string routeId { get; set; }
    public string clusterId { get; set; }
    public string path { get; set; }
}
