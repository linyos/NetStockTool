using System;
using System.Collections.Generic;

namespace MiniStockWidget.Core.Models
{
    /// <summary>
    /// 股票報價資訊模型
    /// </summary>
    public class StockQuote
    {
        /// <summary>
        /// 股票代號
        /// </summary>
        public string Symbol { get; set; } = string.Empty;

        /// <summary>
        /// 公司名稱
        /// </summary>
        public string CompanyName { get; set; } = string.Empty;

        /// <summary>
        /// 最新價格
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 漲跌金額
        /// </summary>
        public decimal Change { get; set; }

        /// <summary>
        /// 漲跌幅度（%）
        /// </summary>
        public decimal ChangePercent { get; set; }

        /// <summary>
        /// 更新時間
        /// </summary>
        public DateTime UpdateTime { get; set; }

        /// <summary>
        /// 歷史價格資料
        /// </summary>
        public List<HistoricalPrice> HistoricalPrices { get; set; } = new List<HistoricalPrice>();
    }
}
