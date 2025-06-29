using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MiniStockWidget.Core.Models
{
    /// <summary>
    /// 台灣證券交易所 API 回應基礎模型
    /// </summary>
    public class TwseApiResponse
    {
        /// <summary>
        /// 回應狀態
        /// </summary>
        [JsonPropertyName("stat")]
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// 回應日期
        /// </summary>
        [JsonPropertyName("date")]
        public string Date { get; set; } = string.Empty;

        /// <summary>
        /// 標題
        /// </summary>
        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// 欄位名稱
        /// </summary>
        [JsonPropertyName("fields")]
        public List<string> Fields { get; set; } = new List<string>();

        /// <summary>
        /// 資料內容
        /// </summary>
        [JsonPropertyName("data")]
        public List<List<string>> Data { get; set; } = new List<List<string>>();

        /// <summary>
        /// 備註
        /// </summary>
        [JsonPropertyName("notes")]
        public List<string> Notes { get; set; } = new List<string>();
    }

    /// <summary>
    /// 台灣證券交易所每日股價資料模型
    /// </summary>
    public class TwseDailyStockData
    {
        /// <summary>
        /// 日期 (YYYY/MM/DD 格式)
        /// </summary>
        public string Date { get; set; } = string.Empty;

        /// <summary>
        /// 成交股數
        /// </summary>
        public string TradeVolume { get; set; } = string.Empty;

        /// <summary>
        /// 成交金額
        /// </summary>
        public string TradeValue { get; set; } = string.Empty;

        /// <summary>
        /// 開盤價
        /// </summary>
        public string OpeningPrice { get; set; } = string.Empty;

        /// <summary>
        /// 最高價
        /// </summary>
        public string HighestPrice { get; set; } = string.Empty;

        /// <summary>
        /// 最低價
        /// </summary>
        public string LowestPrice { get; set; } = string.Empty;

        /// <summary>
        /// 收盤價
        /// </summary>
        public string ClosingPrice { get; set; } = string.Empty;

        /// <summary>
        /// 漲跌價差
        /// </summary>
        public string Change { get; set; } = string.Empty;

        /// <summary>
        /// 成交筆數
        /// </summary>
        public string Transaction { get; set; } = string.Empty;
    }

    /// <summary>
    /// 台灣證券交易所即時報價資料模型 (需要其他 API)
    /// </summary>
    public class TwseRealtimeQuote
    {
        /// <summary>
        /// 股票代號
        /// </summary>
        public string StockCode { get; set; } = string.Empty;

        /// <summary>
        /// 股票名稱
        /// </summary>
        public string StockName { get; set; } = string.Empty;

        /// <summary>
        /// 成交價
        /// </summary>
        public string Price { get; set; } = string.Empty;

        /// <summary>
        /// 漲跌
        /// </summary>
        public string Change { get; set; } = string.Empty;

        /// <summary>
        /// 漲跌幅
        /// </summary>
        public string ChangePercent { get; set; } = string.Empty;

        /// <summary>
        /// 成交量
        /// </summary>
        public string Volume { get; set; } = string.Empty;

        /// <summary>
        /// 時間
        /// </summary>
        public string Time { get; set; } = string.Empty;
    }
}