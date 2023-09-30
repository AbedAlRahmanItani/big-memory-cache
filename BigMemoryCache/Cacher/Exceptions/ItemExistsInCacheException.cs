using System.Runtime.Serialization;

namespace Cacher.Exceptions;

[Serializable]
public class ItemExistsInCacheException : Exception
{
    protected ItemExistsInCacheException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }

    public ItemExistsInCacheException(string key)
        : base($"An Item with Key '{key}' already exists in the cache.")
    {
    }
}
