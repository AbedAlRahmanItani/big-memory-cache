# Welcome to Big Memory Cache!

Our application – **LUSID** – handles large volumes of data, and also needs to be fast. One strategy to
improve the execution speed of our code is to cache regularly-used data in memory.
This component can store arbitrary types of objects, which are added and retrieved using a unique key.
To avoid the risk of running out of memory, the `CacheProvider` class takes a parameter in its constructor as a configurable threshold for the maximum number of items which the cache can hold at any one time.
If the cache becomes full, any attempts to add additional items should succeed, but will result in another item in the cache being evicted. The cache should implement the ‘least recently used’ approach when selecting which item to evict.
The `CacheProvider` class has a `CachedItemEvictedEvent` event which allows the consumer to know when
items get evicted.

## DI Registration

The consumer should register the `ICacheProvider` interface as follows:
`var maxItemsCountThreshold = 1000; // Get from a configuration file or somewhere else...
IServiceCollection serviceCollection = new ServiceCollection();
serviceCollection.AddSingleton<ICacheProvider>(new CacheProvider(maxItemsCountThreshold));`