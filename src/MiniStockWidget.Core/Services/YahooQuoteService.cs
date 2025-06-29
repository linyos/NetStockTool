using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MiniStockWidget.Core.Models;

namespace MiniStockWidget.Core.Services
{
    /// <summary>
    /// Yahoo Finance 股票報價服務實作
    /// </summary>
    public class YahooQuoteService : IQuoteService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<YahooQuoteService> _logger;
        private readonly int _maxRetryCount = 3;

        public YahooQuoteService(IHttpClientFactory httpClientFactory, ILogger<YahooQuoteService> logger)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// 取得單一股票報價
        /// </summary>
        public async Task<StockQuote> GetQuoteAsync(string symbol, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(symbol))
            {
                throw new ArgumentException("Symbol cannot be empty", nameof(symbol));
            }

            int retryCount = 0;
            Exception? lastException = null;

            while (retryCount < _maxRetryCount)
            {
                try
                {
                    using var httpClient = _httpClientFactory.CreateClient("Yahoo");

                    // 實際 API 呼叫 - 這裡是範例
                    // var response = await httpClient.GetAsync($"v8/finance/quote?symbols={symbol}", cancellationToken);
                    // response.EnsureSuccessStatusCode();
                    // var content = await response.Content.ReadAsStringAsync(cancellationToken);
                    // var result = JsonSerializer.Deserialize<YahooResponse>(content);

                    // 模擬資料 - 實際實作需要解析 Yahoo Finance API 回傳的 JSON
                    return new StockQuote
                    {
                        Symbol = symbol,
                        CompanyName = $"{symbol} Corp",
                        Price = 100.0m,
                        Change = 2.5m,
                        ChangePercent = 2.5m,
                        UpdateTime = DateTime.Now
                    };
                }
                catch (Exception ex)
                {
                    lastException = ex;
                    retryCount++;
                    _logger.LogWarning(ex, "Error fetching quote for {Symbol}. Retry {RetryCount}/{MaxRetry}",
                        symbol, retryCount, _maxRetryCount);

                    if (retryCount < _maxRetryCount)
                    {
                        // 指數退避 (Exponential backoff)
                        await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, retryCount)), cancellationToken);
                    }
                }
            }

            throw new Exception($"Failed to fetch quote for {symbol} after {_maxRetryCount} retries", lastException);
        }

        /// <summary>
        /// 取得多個股票報價
        /// </summary>
        public async Task<IEnumerable<StockQuote>> GetQuotesAsync(IEnumerable<string> symbols, CancellationToken cancellationToken = default)
        {
            var tasks = new List<Task<StockQuote>>();
            foreach (var symbol in symbols)
            {
                tasks.Add(GetQuoteAsync(symbol, cancellationToken));
            }

            return await Task.WhenAll(tasks);
        }

        /// <summary>
        /// 取得股票歷史資料
        /// </summary>
        public async Task<StockQuote> GetHistoricalDataAsync(string symbol, int days = 30, CancellationToken cancellationToken = default)
        {
            var quote = await GetQuoteAsync(symbol, cancellationToken);

            int retryCount = 0;
            Exception? lastException = null;

            while (retryCount < _maxRetryCount)
            {
                try
                {


                    using var httpClient = _httpClientFactory.CreateClient("Yahoo");

                    // 實際 API 呼叫 - 這裡是範例
                    // var endDate = DateTimeOffset.UtcNow;
                    // var startDate = endDate.AddDays(-days);
                    // var response = await httpClient.GetAsync(
                    //     $"v8/finance/chart/{symbol}?period1={startDate.ToUnixTimeSeconds()}&period2={endDate.ToUnixTimeSeconds()}&interval=1d", 
                    //     cancellationToken);
                    // response.EnsureSuccessStatusCode();
                    // var content = await response.Content.ReadAsStringAsync(cancellationToken);
                    // 解析歷史資料 JSON 並添加到 quote.HistoricalPrices

                    // 模擬歷史資料
                    var random = new Random();
                    var basePrice = 100.0m;
                    var prices = new List<HistoricalPrice>();

                    for (int i = 0; i < days; i++)
                    {
                        var date = DateTime.Now.AddDays(-i);
                        var price = basePrice + (decimal)(random.NextDouble() * 10 - 5);
                        var volume = random.Next(100000, 10000000);

                        prices.Add(new HistoricalPrice
                        {
                            Timestamp = date,
                            Price = price,
                            Volume = volume
                        });
                    }

                    quote.HistoricalPrices = prices;
                    return quote;
                }
                catch (Exception ex)
                {
                    lastException = ex;
                    retryCount++;
                    _logger.LogWarning(ex, "Error fetching historical data for {Symbol}. Retry {RetryCount}/{MaxRetry}",
                        symbol, retryCount, _maxRetryCount);

                    if (retryCount < _maxRetryCount)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, retryCount)), cancellationToken);
                    }
                }
            }

            throw new Exception($"Failed to fetch historical data for {symbol} after {_maxRetryCount} retries", lastException);
        }
    }
}
