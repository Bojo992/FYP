using System.Collections.Concurrent;

namespace Destination_server.DAL;

public class DBService
{
    private readonly ConcurrentDictionary<string, UserModel> _connection = new();
    public ConcurrentDictionary<string, UserModel> Connection => _connection;
}