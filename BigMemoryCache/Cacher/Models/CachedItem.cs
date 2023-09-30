namespace Cacher.Models;

internal class CachedItem
{
    internal string Key { get; set; }

    internal object? Value { get; set; }

    internal DateTime LastTimeUsed { get; set; }
}
