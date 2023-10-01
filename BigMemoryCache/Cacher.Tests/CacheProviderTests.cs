using AutoFixture;
using Cacher.Exceptions;
using FluentAssertions;
using Xunit;

namespace Cacher.Tests;

public class CacheProviderTests
{
    private readonly Fixture _fixture = new();
    private ICacheProvider? _cacheProvider;

    [Fact]
    public void When_AddOrUpdate_AndItemAlreadyExists_ThenShouldUpdateItemSuccessfullyInCache()
    {
        // Arrange
        const int maxItemsCountThreshold = 2;
        var key = _fixture.Create<string>();
        var value1 = _fixture.Create<decimal>();
        var value2 = _fixture.Create<int>();
        _cacheProvider = new CacheProvider(maxItemsCountThreshold);

        // Act
        _cacheProvider.AddOrUpdate(key, value1);
        _cacheProvider.AddOrUpdate(key, value2);

        // Assert
        var expectedValue = _cacheProvider.Get(key);
        expectedValue.Should().Be(value2);
    }

    [Fact]
    public void When_AddOrUpdate_AndItemDoesNotExists_ThenShouldAddItemSuccessfullyToCache()
    {
        // Arrange
        const int maxItemsCountThreshold = 1;
        var key = _fixture.Create<string>();
        var value = _fixture.Create<decimal>();
        _cacheProvider = new CacheProvider(maxItemsCountThreshold);

        // Act
        _cacheProvider.AddOrUpdate(key, value);

        // Assert
        var expectedValue = _cacheProvider.Get(key);
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
        _cacheProvider = new CacheProvider(maxItemsCountThreshold);
        _cacheProvider.CachedItemEvictedEvent += delegate (string key, object? value)
        {
            evictedKey = key;
            evictedValue = value;
        };

        // Act
        _cacheProvider.AddOrUpdate(key1, value1);
        Thread.Sleep(100);
        _cacheProvider.AddOrUpdate(key2, value2);

        // Assert
        var expectedValue = _cacheProvider.Get(key2);
        expectedValue.Should().Be(value2);
        evictedKey.Should().Be(key1);
        evictedValue.Should().Be(value1);
    }

    [Fact]
    public void When_Remove_AndItemDoesNotExist_ThenShouldThrowItemNotFoundException()
    {
        // Arrange
        const int maxItemsCountThreshold = 1;
        var key = _fixture.Create<string>();
        _cacheProvider = new CacheProvider(maxItemsCountThreshold);

        // Act
        var action = () =>_cacheProvider.Remove(key);

        // Assert
        action.Should()
            .Throw<ItemNotFoundException>($"An Item with Key '{key}' was not found in the cache.");
    }

    [Fact]
    public void When_Remove_AndItemExistsInCache_ThenShouldRemoveItemSuccessfullyFromCache()
    {
        // Arrange
        const int maxItemsCountThreshold = 1;
        var key = _fixture.Create<string>();
        var value = _fixture.Create<decimal>();
        _cacheProvider = new CacheProvider(maxItemsCountThreshold);

        // Act
        _cacheProvider.AddOrUpdate(key, value);
        _cacheProvider.Remove(key);

        // Assert
        var action = () => _cacheProvider.Get(key);
        action.Should()
            .Throw<ItemNotFoundException>($"An Item with Key '{key}' was not found in the cache.");
    }
}