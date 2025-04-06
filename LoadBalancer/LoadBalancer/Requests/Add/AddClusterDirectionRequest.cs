namespace LoadBalancer.requests;

public class AddClusterDirectionRequest
{
    public string directionPath { get; set; }
    public string directionName { get; set; }
    public string clusterId { get; set; }
    public string? originalDirectionName { get; set; }
}