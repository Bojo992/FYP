namespace LoadBalancer.requests;

public class UpdateLoadBalancingPolicyRequest
{
    public string clusterId { get; set; }
    public string loadBalancingPolicy { get; set; }
}