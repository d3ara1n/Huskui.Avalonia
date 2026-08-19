namespace Huskui.Avalonia.Mvvm.States;

internal sealed class DefaultViewStateStore(IViewStatePersistence persistence) : IViewStateStore
{
    private sealed class Entry(Type type, object value)
    {
        public Type Type { get; } = type;
        public object Value { get; } = value;
        public int RefCount { get; set; } = 1;
    }

    private readonly Dictionary<string, Entry> _entries = new();

    public object GetOrCreate(string key, Type stateType)
    {
        if (_entries.TryGetValue(key, out var existing))
        {
            existing.RefCount++;
            return existing.Value;
        }

        var loaded = persistence.Load(key, stateType);
        var value =
            loaded
            ?? Activator.CreateInstance(stateType)
            ?? throw new InvalidOperationException($"Cannot create {stateType.FullName}.");
        _entries[key] = new(stateType, value);
        return value;
    }

    public void Release(string key)
    {
        if (!_entries.TryGetValue(key, out var entry))
        {
            return;
        }

        entry.RefCount--;
        if (entry.RefCount <= 0)
        {
            persistence.Save(key, entry.Type, entry.Value);
            _entries.Remove(key);
        }
    }

    public void Flush()
    {
        foreach (var pair in _entries)
        {
            persistence.Save(pair.Key, pair.Value.Type, pair.Value.Value);
        }
        _entries.Clear();
    }
}
