using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MiniStockWidget.Core.Models;

namespace MiniStockWidget.Core.Services
{
    /// <summary>
    /// 股票報價服務介面
    /// </summary>
    public interface IQuoteService
    {
        /// <summary>
        /// 取得單一股票報價
        /// </summary>
        /// <param name="symbol">股票代號</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>股票報價資訊</returns>
        Task<StockQuote> GetQuoteAsync(string symbol, CancellationToken cancellationToken = default);

        /// <summary>
        /// 取得多個股票報價
        /// </summary>
        /// <param name="symbols">股票代號列表</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>股票報價資訊列表</returns>
        Task<IEnumerable<StockQuote>> GetQuotesAsync(IEnumerable<string> symbols, CancellationToken cancellationToken = default);

        /// <summary>
        /// 取得股票歷史資料
        /// </summary>
        /// <param name="symbol">股票代號</param>
        /// <param name="days">天數</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>包含歷史資料的股票報價資訊</returns>
        Task<StockQuote> GetHistoricalDataAsync(string symbol, int days = 30, CancellationToken cancellationToken = default);
    }
}
