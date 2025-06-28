using FluentAssertions;
using Microsoft.Extensions.Logging;
using MiniStockWidget.Core.Cache;
using MiniStockWidget.Core.Models;
using MiniStockWidget.Core.Services;

namespace MiniStockWidget.Tests;

public class StockQuoteTests
{
    [Fact]
    public void StockQuote_ShouldCreateWithDefaultValues()
    {
        // Arrange & Act
        var quote = new StockQuote();

        // Assert
        quote.Symbol.Should().BeEmpty();
        quote.CompanyName.Should().BeEmpty();
        quote.Price.Should().Be(0);
        quote.Change.Should().Be(0);
        quote.ChangePercent.Should().Be(0);
        quote.HistoricalPrices.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void StockQuote_ShouldSetPropertiesCorrectly()
    {
        // Arrange
        var quote = new StockQuote
        {
            Symbol = "TSLA",
            CompanyName = "Tesla Inc.",
            Price = 250.50m,
            Change = 5.25m,
            ChangePercent = 2.14m,
            UpdateTime = DateTime.Now
        };

        // Assert
        quote.Symbol.Should().Be("TSLA");
        quote.CompanyName.Should().Be("Tesla Inc.");
        quote.Price.Should().Be(250.50m);
        quote.Change.Should().Be(5.25m);
        quote.ChangePercent.Should().Be(2.14m);
    }
}

public class QuoteCacheTests
{
    private readonly ILogger<QuoteCache> _logger;
    private readonly QuoteCache _cache;

    public QuoteCacheTests()
    {
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        _logger = loggerFactory.CreateLogger<QuoteCache>();
        _cache = new QuoteCache(_logger);
    }

    [Fact]
    public void TryGetQuote_WithEmptySymbol_ShouldReturnFalse()
    {
        // Act
        var result = _cache.TryGetQuote("", out var quote);

        // Assert
        result.Should().BeFalse();
        quote.Should().BeNull();
    }

    [Fact]
    public void SetQuote_AndTryGetQuote_ShouldReturnCachedQuote()
    {
        // Arrange
        var testQuote = new StockQuote
        {
            Symbol = "TEST",
            CompanyName = "Test Company",
            Price = 100m,
            Change = 2m,
            ChangePercent = 2m,
            UpdateTime = DateTime.Now
        };

        // Act
        _cache.SetQuote(testQuote);
        var result = _cache.TryGetQuote("TEST", out var cachedQuote);

        // Assert
        result.Should().BeTrue();
        cachedQuote.Should().NotBeNull();
        cachedQuote!.Symbol.Should().Be("TEST");
        cachedQuote.CompanyName.Should().Be("Test Company");
        cachedQuote.Price.Should().Be(100m);
    }

    [Fact]
    public void Clear_ShouldRemoveSpecificQuote()
    {
        // Arrange
        var testQuote = new StockQuote
        {
            Symbol = "TEST",
            Price = 100m
        };

        _cache.SetQuote(testQuote);

        // Act
        _cache.Clear("TEST");
        var result = _cache.TryGetQuote("TEST", out var quote);

        // Assert
        result.Should().BeFalse();
        quote.Should().BeNull();
    }
}