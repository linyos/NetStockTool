using System;

namespace MiniStockWidget.Core.Models
{
    /// <summary>
    /// 歷史價格資料模型
    /// </summary>
    public class HistoricalPrice
    {
        /// <summary>
        /// 時間點
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// 價格
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 成交量
        /// </summary>
        public long Volume { get; set; }
    }
}
