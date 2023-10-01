using AutoFixture;
using FluentAssertions;
using Xunit;

namespace Cacher.Tests;

public class CacheProviderTests
{
    private readonly Fixture _fixture = new();

    [Fact]
    public void When_AddOrUpdate_AndItemAlreadyExists_ThenShouldUpdateItemSuccessfullyInCache()
    {
        // Arrange
        const int maxItemsCountThreshold = 2;
        var key = _fixture.Create<string>();
        var value1 = _fixture.Create<decimal>();
        var value2 = _fixture.Create<int>();
        var cacheProvider = new CacheProvider(maxItemsCountThreshold);

        // Act
        cacheProvider.AddOrUpdate(key, value1);
        cacheProvider.AddOrUpdate(key, value2);

        // Assert
        var expectedValue = cacheProvider.Get(key);
        expectedValue.Should().Be(value2);
    }

    [Fact]
    public void When_AddOrUpdate_AndItemDoesNotExists_ThenShouldAddItemSuccessfullyToCache()
    {
        // Arrange
        const int maxItemsCountThreshold = 1;
        var key = _fixture.Create<string>();
        var value = _fixture.Create<decimal>();
        var cacheProvider = new CacheProvider(maxItemsCountThreshold);

        // Act
        cacheProvider.AddOrUpdate(key, value);

        // Assert
        var expectedValue = cacheProvider.Get(key);
        expectedValue.Should().Be(value);
    }

    [Fact]
    public void When_AddOrUpdate_AndItemDoesNotExists_AndCachedItemsCountReachedThreshold_ThenShouldAddItemAndEvictLeastUsedItem()
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
        cacheProvider.AddOrUpdate(key1, value1);
        Thread.Sleep(100);
        cacheProvider.AddOrUpdate(key2, value2);

        // Assert
        var expectedValue = cacheProvider.Get(key2);
        expectedValue.Should().Be(value2);
        evictedKey.Should().Be(key1);
        evictedValue.Should().Be(value1);
    }
}