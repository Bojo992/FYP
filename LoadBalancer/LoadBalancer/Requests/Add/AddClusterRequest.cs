namespace LoadBalancer.requests;

public class AddClusterRequest
{
    public string clusterId { get; set; }
    public string directionName { get; set; }
    public string directionPath { get; set; }
    
    public string loadBalancingPolicy { get; set; }
}