using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using MiniStockWidget.Core.Models;
using Microsoft.Extensions.Logging;

namespace MiniStockWidget.Core.Cache
{
    /// <summary>
    /// 記憶體內報價快取
    /// </summary>
    public class QuoteCache
    {
        private readonly ConcurrentDictionary<string, StockQuote> _quoteCache = new();
        private readonly ConcurrentDictionary<string, DateTime> _lastUpdateTimes = new();
        private readonly ILogger<QuoteCache> _logger;
        private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(5);

        public QuoteCache(ILogger<QuoteCache> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// 取得快取的報價
        /// </summary>
        public bool TryGetQuote(string symbol, out StockQuote? quote)
        {
            quote = null;
            if (string.IsNullOrWhiteSpace(symbol))
            {
                return false;
            }

            if (_quoteCache.TryGetValue(symbol, out var cachedQuote) &&
                _lastUpdateTimes.TryGetValue(symbol, out var lastUpdate))
            {
                if (DateTime.Now - lastUpdate < _cacheDuration)
                {
                    quote = cachedQuote;
                    return true;
                }

                // 快取已過期，移除
                _logger.LogInformation("Cache expired for {Symbol}", symbol);
                _quoteCache.TryRemove(symbol, out _);
                _lastUpdateTimes.TryRemove(symbol, out _);
            }

            return false;
        }

        /// <summary>
        /// 設定報價至快取
        /// </summary>
        public void SetQuote(StockQuote quote)
        {
            if (quote == null || string.IsNullOrWhiteSpace(quote.Symbol))
            {
                return;
            }

            _quoteCache[quote.Symbol] = quote;
            _lastUpdateTimes[quote.Symbol] = DateTime.Now;
            _logger.LogInformation("Added/updated cache for {Symbol}", quote.Symbol);
        }

        /// <summary>
        /// 設定多個報價至快取
        /// </summary>
        public void SetQuotes(IEnumerable<StockQuote> quotes)
        {
            if (quotes == null)
            {
                return;
            }

            foreach (var quote in quotes)
            {
                SetQuote(quote);
            }
        }

        /// <summary>
        /// 清除所有快取
        /// </summary>
        public void Clear()
        {
            _quoteCache.Clear();
            _lastUpdateTimes.Clear();
            _logger.LogInformation("Cleared all cache entries");
        }

        /// <summary>
        /// 清除特定報價的快取
        /// </summary>
        public void Clear(string symbol)
        {
            if (string.IsNullOrWhiteSpace(symbol))
            {
                return;
            }

            _quoteCache.TryRemove(symbol, out _);
            _lastUpdateTimes.TryRemove(symbol, out _);
            _logger.LogInformation("Cleared cache for {Symbol}", symbol);
        }
    }
}