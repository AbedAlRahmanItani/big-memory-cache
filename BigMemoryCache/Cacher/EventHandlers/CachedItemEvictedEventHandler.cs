namespace Cacher.EventHandlers;

public delegate void CachedItemEvictedEventHandler(string cachedItemKey, object? cachedItem);