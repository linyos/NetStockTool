using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MiniStockWidget.Core.Models;

namespace MiniStockWidget.Core.Services
{

    /// <summary>
    /// 台灣證券交易所 (TWSE) 股票報價服務實作
    /// </summary>
    public class TwseQuoteService : IQuoteService
    {



        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<TwseQuoteService> _logger;

        private readonly int _maxRetryCount = 3;

        public TwseQuoteService(IHttpClientFactory httpClientFactory, ILogger<TwseQuoteService> logger)
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

            var quote = await GetHistoricalDataAsync(symbol, 1, cancellationToken);
            return quote;
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
                    var stockNo = symbol.Replace(".TW", "").Replace(".tw", ""); // 處理 TWSE 股票代號格式

                    var currentDate = DateTime.Now;
                    var startDate = currentDate.ToString("yyyyMMdd");


                    var url = $"https://www.twse.com.tw/exchangeReport/STOCK_DAY?response=json&stockNo={stockNo}&date={startDate}";


                    using var httpClient = _httpClientFactory.CreateClient("TWSE");
                    httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
                    // api get stock data
                    var response = await httpClient.GetStringAsync(url);
                    // convert json to object
                    var apiResponse = JsonSerializer.Deserialize<TwseApiResponse>(response);

                    if (apiResponse == null || apiResponse.Status != "OK" || apiResponse.Data.Count == 0)
                    {
                        _logger.LogWarning("No data returned for symbol {Symbol}", symbol);
                        return CreateEmptyQuote(symbol);
                    }
                    // 轉換 API 資料為內部模型
                    var historicalPrices = ConvertToHistoricalPrices(apiResponse);
                    var latestPrice = historicalPrices.LastOrDefault();
                    if (latestPrice == null)
                    {
                        return CreateEmptyQuote(symbol);
                    }

                    // 計算漲跌
                    var previousPrice = historicalPrices.Count > 1 ? historicalPrices[historicalPrices.Count - 2].Price : latestPrice.Price;
                    var change = latestPrice.Price - previousPrice;
                    var priceChangePercent = previousPrice != 0 ? change / previousPrice * 100 : 0;

                    var quote = new StockQuote
                    {
                        Symbol = symbol,
                        CompanyName = GetCompanyName(stockNo),
                        Price = latestPrice.Price,
                        Change = change,
                        ChangePercent = priceChangePercent,
                        UpdateTime = latestPrice.Timestamp,
                        HistoricalPrices = historicalPrices.Take(days).ToList()
                    };
                    _logger.LogInformation("Successfully fetched data for {Symbol}: Price={Price}, Change={Change}",
                        symbol, quote.Price, quote.Change);

                    return quote;
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
            throw new Exception($"Failed to fetch historical data for {symbol} after {_maxRetryCount} retries", lastException);
        }
        /// <summary>
        /// 轉換 TWSE API 資料為歷史價格列表
        /// </summary>
        private List<HistoricalPrice> ConvertToHistoricalPrices(TwseApiResponse apiResponse)
        {
            var prices = new List<HistoricalPrice>();
            foreach (var data in apiResponse.Data)
            {
                try
                {
                    if (data.Count < 9) continue; // 確保資料完整
                                                  // TWSE API 欄位順序: 日期,成交股數,成交金額,開盤價,最高價,最低價,收盤價,漲跌價差,成交筆數

                    var dateStr = data[0].Replace("/", "-"); //將 111/01/03 轉換為 111-01-03
                    var closeingPriceStr = data[6].ToString();
                    var volumeStr = data[1];


                    // 轉換民國年為西元年
                    if (DateTime.TryParseExact($"20{dateStr}", "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
                    {
                        // 如果是民國年格式 (111/01/03)，需要加上 1911
                        var parts = data[0].Split('/');
                        if (parts.Length == 3 && int.TryParse(parts[0], out var rocYear) && rocYear < 200)
                        {
                            var year = rocYear + 1911;
                            date = new DateTime(year, int.Parse(parts[1]), int.Parse(parts[2]));
                        }
                    }

                    if (decimal.TryParse(closeingPriceStr, out var price) &&
                        long.TryParse(volumeStr.Replace(",", ""), out var volume))
                    {
                        prices.Add(new HistoricalPrice
                        {
                            Timestamp = date,
                            Price = price,
                            Volume = volume
                        });
                    }



                }
                catch (System.Exception ex)
                {
                    _logger.LogWarning(ex, "Error parsing data row: {Row}", string.Join(",", data));
                }

            }
            return prices.OrderBy(p => p.Timestamp).ToList();
        }


        /// <summary>
        /// 建立空的股票報價
        /// </summary>
        private StockQuote CreateEmptyQuote(string symbol)
        {
            return new StockQuote
            {
                Symbol = symbol,
                CompanyName = GetCompanyName(symbol),
                Price = 0,
                Change = 0,
                ChangePercent = 0,
                UpdateTime = DateTime.Now,
                HistoricalPrices = new List<HistoricalPrice>()
            };

        }


        /// <summary>
        /// 取得公司名稱 (簡化版本，實際應該查詢資料庫或另一個 API)
        /// </summary>
        private string GetCompanyName(string stockCode)
        {
            var commonStocks = new Dictionary<string, string>
            {
                { "0050", "元大台灣50" },
                { "2330", "台積電" },
                { "2454", "聯發科" },
                { "2412", "中華電" },
                { "2317", "鴻海" },
                { "1301", "台塑" },
                { "1303", "南亞" },
                { "2881", "富邦金" },
                { "2882", "國泰金" },
                { "2884", "玉山金" }
            };
            return commonStocks.TryGetValue(stockCode, out var name) ? name : $"股票代號 {stockCode} 的公司名稱未知";
        }
    }
}