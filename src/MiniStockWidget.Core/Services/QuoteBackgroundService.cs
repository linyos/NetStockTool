using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using MiniStockWidget.Core.Models;
using MiniStockWidget.Core.Services;

namespace MiniStockWidget.Core.Services
{
    /// <summary>
    /// 股票報價背景服務
    /// </summary>
    public class QuoteBackgroundService : BackgroundService
    {

        // 日誌記錄

        private readonly ILogger<QuoteBackgroundService> _logger;
        // 服務提供者
        private readonly IServiceProvider _serviceProvider;

        // 更新間隔，默認為30秒
        private TimeSpan _updateInterval = TimeSpan.FromSeconds(30);

        // 監視清單，默認包含一些熱門股票
        private List<string> _watchlist = new List<string> { "2330.TW", "0050.TW" };

        /// <summary>
        /// 最新報價更新事件
        /// </summary>
        public event EventHandler<List<StockQuote>>? QuotesUpdated;

        public QuoteBackgroundService(IServiceProvider serviceProvider, ILogger<QuoteBackgroundService> logger)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// 設定更新間隔
        /// </summary>
        public void SetUpdateInterval(TimeSpan interval)
        {
            // 設定更新間隔
            if (interval.TotalSeconds < 5)
            {
                throw new ArgumentException("Update interval cannot be less than 5 seconds", nameof(interval));
            }
            _updateInterval = interval;
            _logger.LogInformation("Update interval set to {Interval} seconds", _updateInterval.TotalSeconds);
        }

        /// <summary>
        /// 設定監視清單
        /// </summary>
        public void SetWatchlist(List<string> symbols)
        {
            if (symbols == null || symbols.Count == 0)
            {
                throw new ArgumentException("Watchlist cannot be empty", nameof(symbols));
            }
            _watchlist = new List<string>(symbols);
            _logger.LogInformation("Watchlist updated: {Symbols}", string.Join(", ", _watchlist));
        }

        /// <summary>
        /// 背景服務執行邏輯
        /// </summary>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Quote background service is starting");

            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    await UpdateQuotesAsync(stoppingToken);
                    await Task.Delay(_updateInterval, stoppingToken);
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Quote background service was cancelled");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in the quote background service");
            }
            finally
            {
                _logger.LogInformation("Quote background service is stopping");
            }
        }

        /// <summary>
        /// 更新報價資料
        /// </summary>
        private async Task UpdateQuotesAsync(CancellationToken cancellationToken)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var quoteService = scope.ServiceProvider.GetRequiredService<IQuoteService>();

                var quotes = await quoteService.GetQuotesAsync(_watchlist, cancellationToken);
                var quotesList = new List<StockQuote>(quotes);

                _logger.LogInformation("Fetched {Count} quotes", quotesList.Count);

                // 觸發更新事件
                QuotesUpdated?.Invoke(this, quotesList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating quotes");
            }
        }
    }
}
