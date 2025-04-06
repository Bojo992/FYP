namespace LoadBalancer.requests;

public class UpdateClusterDirectionRequest
{
    public string clusterId { get; set; }
    public string directionPath { get; set; }
    public string directionName { get; set; }
}