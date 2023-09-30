using Cacher.EventHandlers;
using Cacher.Exceptions;
using Cacher.Models;
using System.Collections.Concurrent;

namespace Cacher;

public class CacheProvider : ICacheProvider
{
    private readonly int _maxItemsCountThreshold;
    private ConcurrentDictionary<string, CachedItem> _cachedItemsDictionary;

    public CacheProvider(int maxItemsCountThreshold)
    {
        _maxItemsCountThreshold = maxItemsCountThreshold;
        _cachedItemsDictionary = new ConcurrentDictionary<string, CachedItem>(StringComparer.OrdinalIgnoreCase);
    }

    public event CachedItemEvictedEventHandler? CachedItemEvictedEvent;

    public object? Get(string key)
    {
        if (!_cachedItemsDictionary.ContainsKey(key))
        {
            throw new ItemNotFoundException(key);
        }

        var cachedItem = _cachedItemsDictionary[key];
        cachedItem.LastTimeUsed = DateTime.UtcNow;

        return cachedItem.Value;
    }

    public void Add(string key, object value)
    {
        if (_cachedItemsDictionary.ContainsKey(key))
        {
            throw new ItemExistsInCacheException(key);
        }

        if (ShouldRemoveLeastRecentlyUsedItem())
        {
            RemoveLeastRecentlyUsedItem();
        }

        var cachedItem = new CachedItem
        {
            Key = key,
            Value = value,
            LastTimeUsed = DateTime.UtcNow
        };
        _cachedItemsDictionary.TryAdd(key, cachedItem);
    }

    public void Remove(string key)
    {
        if (!_cachedItemsDictionary.ContainsKey(key))
        {
            throw new ItemNotFoundException(key);
        }

        _cachedItemsDictionary.TryRemove(key, out _);
    }

    private bool ShouldRemoveLeastRecentlyUsedItem()
    {
        return _cachedItemsDictionary.Keys.Count == _maxItemsCountThreshold;
    }

    private void RemoveLeastRecentlyUsedItem()
    {
        var leastRecentlyUsedItem = _cachedItemsDictionary.Values
            .OrderBy(x => x.LastTimeUsed)
            .FirstOrDefault();

        if (leastRecentlyUsedItem != null)
        {
            _cachedItemsDictionary.TryRemove(leastRecentlyUsedItem.Key, out _);
            RaiseCachedItemEvictedEvent(leastRecentlyUsedItem);
        }
    }

    private void RaiseCachedItemEvictedEvent(CachedItem cachedItem)
    {
        CachedItemEvictedEvent?.Invoke(cachedItem.Key, cachedItem.Value);
    }
}