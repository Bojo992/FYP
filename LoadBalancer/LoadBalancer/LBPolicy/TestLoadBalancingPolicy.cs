using System.Security.Cryptography;
using System.Text;
using Yarp.ReverseProxy.LoadBalancing;
using Yarp.ReverseProxy.Model;

namespace LoadBalancer.LBPolicy;

public sealed class TestLoadBalancingPolicy : ILoadBalancingPolicy
{
    public string Name => "IPHashing";
    
    private Dictionary<string, string> _userDirections = new();
    private Dictionary<string, int> _destinations = new();

    public DestinationState? PickDestination(HttpContext context, ClusterState cluster, IReadOnlyList<DestinationState> availableDestinations)
    {
        checkForNewDestinations(cluster);
       
        var ipAddress = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
       
        if (string.IsNullOrEmpty(ipAddress))
        {
           ipAddress = context.Connection.RemoteIpAddress?.ToString();
        }
        
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(ipAddress);
        var hashBytes = sha256.ComputeHash(bytes);
        var userIp = Convert.ToHexString(hashBytes);

        if (_userDirections.TryGetValue(userIp, out var direction))
        {
            if (cluster.Destinations.TryGetValue(direction, out var destinationState))
            {
                return destinationState;
            }
        }
        else
        {
            var suitableDestinations = _destinations
                .Where(kvp => kvp.Value == _destinations.Min(c => c.Value))
                .Select(kvp => kvp)
                .ToList();
            
            var randomDestination = suitableDestinations[new Random().Next(suitableDestinations.Count)];

            _userDirections.Add(userIp, randomDestination.Key);
            _destinations[randomDestination.Key] = randomDestination.Value + 1;

            if (cluster.Destinations.TryGetValue(randomDestination.Key, out var destinationState))
            {
                return destinationState;
            }
        }
        
        return null; 
    }

    private void checkForNewDestinations(ClusterState availableDestinations)
    {
        var newDestinations = availableDestinations.Destinations.Keys.Except(_destinations.Keys).ToList();

        if (newDestinations.Count != 0)
        {
            newDestinations.ForEach(k => _destinations.TryAdd(k, 0));
        }
    }
}