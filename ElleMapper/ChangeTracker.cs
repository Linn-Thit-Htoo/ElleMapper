using System.Collections.Concurrent;

namespace ElleMapper;

public class ChangeTracker
{
    private readonly ConcurrentDictionary<object, EntityState> _entries = new();

    public void Track(object entity, EntityState state)
    {
        _entries[entity] = state;
    }

    public IReadOnlyDictionary<object, EntityState> Entries()
    {
        return _entries;
    }

    public void Detach(object entity)
    {
        _entries.TryRemove(entity, out var state);
    }
}
