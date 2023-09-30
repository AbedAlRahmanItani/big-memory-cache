using AutoFixture;
using Cacher.Exceptions;
using FluentAssertions;
using Xunit;

namespace Cacher.Tests;

public class CacheProviderTests
{
    private readonly Fixture _fixture = new();

    [Fact]
    public void When_Add_AndItemAlreadyExists_ThenShouldThrowItemExistsInCacheException()
    {
        // Arrange
        const int maxItemsCountThreshold = 2;
        var key = _fixture.Create<string>();
        var value = _fixture.Create<decimal>();
        var cacheProvider = new CacheProvider(maxItemsCountThreshold);

        // Act
        var action = () =>
        {
            cacheProvider.Add(key, value);
            cacheProvider.Add(key, value);
        };

        // Assert
        action.Should()
            .Throw<ItemExistsInCacheException>($"An Item with Key '{key}' already exists in the cache.");
    }

    [Fact]
    public void When_Add_AndItemDoesNotExists_AndCachedItemsCountBelowThreshold_ThenShouldAddItemSuccessfullyToCache()
    {
        // Arrange
        const int maxItemsCountThreshold = 1;
        var key = _fixture.Create<string>();
        var value = _fixture.Create<decimal>();
        var cacheProvider = new CacheProvider(maxItemsCountThreshold);

        // Act
        cacheProvider.Add(key, value);

        // Assert
        var expectedValue = cacheProvider.Get(key);
        expectedValue.Should().Be(value);
    }

    [Fact]
    public void When_Add_AndItemDoesNotExists_AndCachedItemsCountReachesThreshold_ThenShouldAddItemAndEvictLeastUsedItem()
    {
        // Arrange
        const int maxItemsCountThreshold = 1;
        var key1 = _fixture.Create<string>();
        var value1 = _fixture.Create<decimal>();
        var key2 = _fixture.Create<string>();
        var value2 = _fixture.Create<int>();
        string evictedKey = string.Empty;
        object? evictedValue = null;
        var cacheProvider = new CacheProvider(maxItemsCountThreshold);
        cacheProvider.CachedItemEvictedEvent += delegate (string key, object? value)
        {
            evictedKey = key;
            evictedValue = value;
        };

        // Act
        cacheProvider.Add(key1, value1);
        Thread.Sleep(100);
        cacheProvider.Add(key2, value2);

        // Assert
        var expectedValue = cacheProvider.Get(key2);
        expectedValue.Should().Be(value2);
        evictedKey.Should().Be(key1);
        evictedValue.Should().Be(value1);
    }
}