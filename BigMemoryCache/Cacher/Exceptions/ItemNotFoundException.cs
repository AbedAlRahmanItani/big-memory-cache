using System.Runtime.Serialization;

namespace Cacher.Exceptions;

[Serializable]
public class ItemNotFoundException : Exception
{
    protected ItemNotFoundException(SerializationInfo info, StreamingContext context) 
        : base(info, context)
    {
    }

    public ItemNotFoundException(string key)
        : base($"An Item with Key '{key}' was not found in the cache.")
    {
    }
}
