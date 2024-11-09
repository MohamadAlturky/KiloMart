using System.Collections.Concurrent;

namespace KiloMart.Api.RealTime;

public class ConnectionMapping<T>
{
    private readonly ConcurrentDictionary<T, HashSet<string>> _connections =
        new ConcurrentDictionary<T, HashSet<string>>();

    public int Count => _connections.Count;

    public void Add(T key, string connectionId)
    {
        var connections = _connections.GetOrAdd(key, _ => new HashSet<string>());
        lock (connections)
        {
            connections.Add(connectionId);
        }
    }

    public IEnumerable<string> GetConnections(T key)
    {
        if (_connections.TryGetValue(key, out var connections))
        {
            return connections;
        }

        return Enumerable.Empty<string>();
    }

    public void Remove(T key, string connectionId)
    {
        if (_connections.TryGetValue(key, out var connections))
        {
            lock (connections)
            {
                connections.Remove(connectionId);

                if (connections.Count == 0)
                {
                    _connections.TryRemove(key, out _);
                }
            }
        }
    }
}
