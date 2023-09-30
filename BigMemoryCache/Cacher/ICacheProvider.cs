using Cacher.EventHandlers;

namespace Cacher;

public interface ICacheProvider
{
    event CachedItemEvictedEventHandler? CachedItemEvictedEvent;

    object? Get(string key);

    void Add(string key, object item);

    void Remove(string key);
}
