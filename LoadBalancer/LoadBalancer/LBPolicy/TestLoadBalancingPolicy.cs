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
        
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(context.Connection.RemoteIpAddress.ToString());
        var hashBytes = sha256.ComputeHash(bytes);
        var userIp =  Convert.ToHexString(hashBytes);

        if (_userDirections.TryGetValue(userIp, out var direction))
        {
            cluster.Destinations.TryGetValue(direction, out var destinationState);
            return destinationState;
        }
        else
        {
            var suitableDestination = _destinations
                .Where(kvp => kvp.Value == _destinations.Min(c => c.Value))
                .Select(kvp => kvp)
                .OrderBy(k => k.Key)
                .First();
            
            _userDirections.Add(userIp, suitableDestination.Key);
            _destinations.TryAdd(suitableDestination.Key, suitableDestination.Value + 1);
            
            return cluster.Destinations[suitableDestination.Key];
        }
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