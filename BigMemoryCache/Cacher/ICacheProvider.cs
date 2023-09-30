namespace Cacher;

public interface ICacheProvider
{
    object? Get(string key);

    void Add(string key, object item);

    void Remove(string key);
}
