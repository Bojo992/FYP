using LoadBalancer.requests;
using LoadBalancer.Requests.Delete;
using LoadBalancer.Services;
using Microsoft.AspNetCore.Mvc;

namespace LoadBalancer;

[ApiController]
public class ConfigController : Controller
{
    private IConfigService _service;

    public ConfigController(IConfigService service)
    {
        _service = service;
    }

    [HttpGet("get-config")]
    public IActionResult GetConfig()
    {
        return Ok(_service.GetConfig());
    }
    
    //Add
    [HttpPost("add-cluster")]
    public IActionResult AddCluster([FromBody] AddClusterRequest request)
    {
        return Ok(_service.AddCluster(request));
    }
    
    [HttpPost("add-direction")]
    public IActionResult AddDirection([FromBody] AddClusterDirectionRequest request)
    {
        return Ok(_service.AddClusterDirection(request));
    }

    [HttpPost("add-route")]
    public IActionResult AddRoute([FromBody] AddRouteRequest request)
    {
        return Ok(_service.AddRoute(request));
    }
    
    //Update
    [HttpPost("/update-cluster-load-balancing-policy")]
    public IActionResult UpdateClusterLoadBalancingPolicy([FromBody] UpdateLoadBalancingPolicyRequest policy)
    {
        return Ok(_service.UpdateClusterLoadBalancingPolicy(policy));
    }

    [HttpPost("update-cluster-directions")]
    public IActionResult UpdateClusterDirections([FromBody] AddClusterDirectionRequest request)
    {
        return Ok(_service.UpdateClusterDirections(request));
    }

    [HttpPost("update-route")]
    public IActionResult UpdateRoute([FromBody] UpdateRouteRequest route)
    {
        return Ok(_service.UpdateRoute(route));
    }


    //Remove
    [HttpDelete("remove-cluster")]
    public IActionResult DeleteCluster([FromBody] DeleteClusterRequest cluster)
    {
        return Ok(_service.DeleteCluster(cluster));
    }

    [HttpDelete("remove-direction")]
    public IActionResult DeleteDirection([FromBody] DeleteClusterDirectionRequest cluster)
    {
        return Ok(_service.DeleteClusterDirections(cluster));
    }
    
    [HttpDelete("remove-route")]
    public IActionResult DeleteRoute([FromBody] DeleteRouteRequest route)
    {
        return Ok(_service.DeleteRoute(route));
    }
}