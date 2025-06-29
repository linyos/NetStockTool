using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using MiniStockWidget.Core.Models;
using MiniStockWidget.Core.Services;
using Microsoft.Extensions.Logging;

namespace MiniStockView.ViewModels
{
    /// <summary>
    /// 主視窗的 ViewModel
    /// </summary>
    public partial class MainViewModel : ObservableObject
    {
        private readonly IQuoteService _quoteService;
        private readonly ILogger<MainViewModel> _logger;
        private readonly DispatcherTimer _timeUpdateTimer;
        private List<HistoricalPrice> _historicalPrices = new();

        [ObservableProperty]
        private string _symbol = "0050.TW"; // 預設股票代號

        [ObservableProperty]
        private string _companyName = "元大台灣50";

        [ObservableProperty]
        private decimal _currentPrice = 0;

        [ObservableProperty]
        private decimal _priceChange = 0;

        [ObservableProperty]
        private decimal _changePercent = 0;

        [ObservableProperty]
        private DateTime _lastUpdate = DateTime.Now;

        [ObservableProperty]
        private string _currentTime = DateTime.Now.ToString("HH:mm:ss");

        [ObservableProperty]
        private bool _isLoading = false;

        [ObservableProperty]
        private string _statusMessage = "準備就緒";

        /// <summary>
        /// 圖表系列數據
        /// </summary>
        public ObservableCollection<ISeries> Series { get; } = new();

        /// <summary>
        /// X 軸配置
        /// </summary>
        public Axis[] XAxes { get; private set; } = default!;

        /// <summary>
        /// Y 軸配置
        /// </summary>
        public Axis[] YAxes { get; private set; } = default!;

        /// <summary>
        /// 價格變化的顏色（用於綁定UI）
        /// </summary>
        public string PriceChangeColor => PriceChange >= 0 ? "#F44336" : "#4CAF50";

        /// <summary>
        /// 變化箭頭圖標
        /// </summary>
        public string ChangeIcon => PriceChange >= 0 ? "▲" : "▼";

        /// <summary>
        /// 刷新命令
        /// </summary>
        public ICommand RefreshCommand { get; }

        /// <summary>
        /// 關閉命令
        /// </summary>
        public ICommand CloseCommand { get; }

        public MainViewModel(IQuoteService quoteService, ILogger<MainViewModel> logger)
        {
            _quoteService = quoteService ?? throw new ArgumentNullException(nameof(quoteService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            RefreshCommand = new AsyncRelayCommand(RefreshDataAsync);
            CloseCommand = new RelayCommand(() => System.Windows.Application.Current.Shutdown());

            // 初始化軸配置
            InitializeAxes();

            // 初始化時間更新定時器
            _timeUpdateTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1) // 每秒更新一次
            };
            _timeUpdateTimer.Tick += OnTimeUpdateTimerTick;
            _timeUpdateTimer.Start();

            InitializeChart();
            _ = Task.Run(RefreshDataAsync);
        }

        /// <summary>
        /// 時間更新定時器事件處理
        /// </summary>
        private void OnTimeUpdateTimerTick(object? sender, EventArgs e)
        {
            CurrentTime = DateTime.Now.ToString("HH:mm:ss");
        }

        /// <summary>
        /// 初始化圖表
        /// </summary>
        private void InitializeChart()
        {
            // 獲取中文字體
            SKTypeface? chineseTypeface = null;
            try
            {
                var fontFamilies = new[] { "Microsoft JhengHei", "Microsoft YaHei", "SimSun", "Arial Unicode MS", "DejaVu Sans" };
                foreach (var fontFamily in fontFamilies)
                {
                    chineseTypeface = SKTypeface.FromFamilyName(fontFamily, SKFontStyle.Normal);
                    if (chineseTypeface != null && chineseTypeface.FamilyName != "")
                        break;
                }
                chineseTypeface ??= SKTypeface.Default;
            }
            catch
            {
                chineseTypeface = SKTypeface.Default;
            }

            var lineSeries = new LineSeries<ObservablePoint>
            {
                Values = new ObservableCollection<ObservablePoint>(),
                Name = "股價走勢",
                GeometrySize = 3,
                GeometryStroke = new SolidColorPaint(SKColors.DeepSkyBlue) { StrokeThickness = 1.5f },
                GeometryFill = new SolidColorPaint(SKColors.White),
                Stroke = new SolidColorPaint(SKColors.DeepSkyBlue) { StrokeThickness = 2 },
                Fill = new LinearGradientPaint(
                    new[] { SKColors.DeepSkyBlue.WithAlpha(80), SKColors.Transparent },
                    new SKPoint(0, 0),
                    new SKPoint(0, 1)
                ),
                LineSmoothness = 0.3, // 使線條更平滑
                EnableNullSplitting = false,
                DataLabelsSize = 0, // 隱藏數據標籤
                DataLabelsPaint = null // 不顯示數據標籤
            };

            Series.Add(lineSeries);
        }

        /// <summary>
        /// 初始化軸配置
        /// </summary>
        private void InitializeAxes()
        {
            // 為中文字體創建字體配置，指定支援中文的字體
            // 提供多種字體備選方案以確保在不同系統上都能正確顯示中文
            SKTypeface? chineseTypeface = null;
            try
            {
                // 嘗試載入常見的中文字體
                var fontFamilies = new[] { "Microsoft JhengHei", "Microsoft YaHei", "SimSun", "Arial Unicode MS", "DejaVu Sans" };
                foreach (var fontFamily in fontFamilies)
                {
                    chineseTypeface = SKTypeface.FromFamilyName(fontFamily, SKFontStyle.Normal);
                    if (chineseTypeface != null && chineseTypeface.FamilyName != "")
                        break;
                }

                // 如果都找不到，使用系統預設字體
                chineseTypeface ??= SKTypeface.Default;
            }
            catch
            {
                chineseTypeface = SKTypeface.Default;
            }

            var chineseFontPaint = new SolidColorPaint(SKColors.White)
            {
                SKTypeface = chineseTypeface
            };
            var chineseLabelPaint = new SolidColorPaint(SKColors.LightGray)
            {
                SKTypeface = chineseTypeface
            };

            XAxes = new Axis[]
            {
                new Axis
                {
                    Name = "",
                    NamePaint = chineseFontPaint,
                    LabelsPaint = chineseLabelPaint,
                    TextSize = 9,
                    SeparatorsPaint = new SolidColorPaint(SKColors.Gray) { StrokeThickness = 0.5f },
                    ShowSeparatorLines = false,
                    Labeler = value =>
                    {
                        // 根據數據點索引顯示時間標籤
                        if (_historicalPrices != null && value >= 0 && value < _historicalPrices.Count)
                        {
                            var index = (int)Math.Round(value);
                            if (index >= 0 && index < _historicalPrices.Count)
                            {
                                // 只顯示部分標籤以避免擁擠
                                var totalPoints = _historicalPrices.Count;
                                var showEvery = Math.Max(1, totalPoints / 6); // 最多顯示 6 個標籤
                                
                                if (index % showEvery == 0 || index == totalPoints - 1)
                                {
                                    // 修正日期時間格式，確保正確顯示
                                    var timestamp = _historicalPrices[index].Timestamp;
                                    return timestamp.ToString("MM/dd");
                                }
                            }
                        }
                        return "";
                    },
                    MinStep = 1,
                    ForceStepToMin = true

                }
            };

            YAxes = new Axis[]
            {
                new Axis
                {
                    Name = "",  // 隱藏軸標題
                    NamePaint = chineseFontPaint,
                    LabelsPaint = null, // 完全隱藏標籤
                    TextSize = 0,  // 設置字體大小為0
                    SeparatorsPaint = new SolidColorPaint(SKColors.Gray.WithAlpha(50)) { StrokeThickness = 0.2f },
                    ShowSeparatorLines = true,
                    IsVisible = true, // 軸線可見但標籤不可見
                    MinLimit = null,
                    MaxLimit = null
                }
            };
        }

        /// <summary>
        /// 刷新股票資料
        /// </summary>
        [RelayCommand]
        private async Task RefreshDataAsync()
        {
            try
            {
                IsLoading = true;
                StatusMessage = "正在更新資料...";

                _logger.LogInformation("Refreshing data for symbol: {Symbol}", Symbol);

                // 獲取股票報價和歷史資料
                var quote = await _quoteService.GetHistoricalDataAsync(Symbol, 30);

                // 更新UI資料
                CompanyName = quote.CompanyName;
                CurrentPrice = quote.Price;
                PriceChange = quote.Change;
                ChangePercent = quote.ChangePercent;
                LastUpdate = quote.UpdateTime;

                // 更新圖表資料
                UpdateChart(quote);

                StatusMessage = $"更新完成 - {LastUpdate:HH:mm:ss}";

                // 通知屬性變更以更新顏色
                OnPropertyChanged(nameof(PriceChangeColor));
                OnPropertyChanged(nameof(ChangeIcon));

                _logger.LogInformation("Data refreshed successfully for {Symbol}", Symbol);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error refreshing data for {Symbol}", Symbol);
                StatusMessage = "更新失敗，請稍後再試";
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary>
        /// 更新圖表資料
        /// </summary>
        private void UpdateChart(StockQuote quote)
        {
            if (quote.HistoricalPrices == null || quote.HistoricalPrices.Count == 0)
            {
                return;
            }

            var lineSeries = Series[0] as LineSeries<ObservablePoint>;
            if (lineSeries?.Values is ObservableCollection<ObservablePoint> values)
            {
                values.Clear();

                // 按時間排序並更新歷史價格數據
                _historicalPrices = quote.HistoricalPrices
                    .OrderBy(p => p.Timestamp)
                    .ToList();

                // 添加到圖表
                for (int i = 0; i < _historicalPrices.Count; i++)
                {
                    var price = _historicalPrices[i];
                    values.Add(new ObservablePoint(i, (double)price.Price));
                }

                // 優化 Y 軸範圍
                if (_historicalPrices.Count > 0)
                {
                    var prices = _historicalPrices.Select(p => (double)p.Price).ToList();
                    var minPrice = prices.Min();
                    var maxPrice = prices.Max();
                    var range = maxPrice - minPrice;
                    var padding = range * 0.1; // 10% 的上下留白

                    YAxes[0].MinLimit = Math.Max(0, minPrice - padding);
                    YAxes[0].MaxLimit = maxPrice + padding;
                }

                _logger.LogInformation("Chart updated with {Count} data points", values.Count);
            }
        }

        /// <summary>
        /// 設定股票代號
        /// </summary>
        public async Task SetSymbolAsync(string symbol)
        {
            if (string.IsNullOrWhiteSpace(symbol) || symbol == Symbol)
            {
                return;
            }

            Symbol = symbol.ToUpper();
            await RefreshDataAsync();
        }
    }
}
