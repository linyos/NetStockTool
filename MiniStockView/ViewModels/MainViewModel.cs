using System;
using System.Collections.ObjectModel;
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
using System.Threading.Tasks;

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

        [ObservableProperty]
        private string _symbol = "TSLA";

        [ObservableProperty]
        private string _companyName = "Tesla Inc.";

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
        public Axis[] XAxes { get; } = new Axis[]
        {
            new Axis
            {
                Name = "時間",
                NamePaint = new SolidColorPaint(SKColors.White),
                LabelsPaint = new SolidColorPaint(SKColors.LightGray),
                TextSize = 10,
                SeparatorsPaint = new SolidColorPaint(SKColors.Gray) { StrokeThickness = 1 },
                ShowSeparatorLines = true
            }
        };

        /// <summary>
        /// Y 軸配置
        /// </summary>
        public Axis[] YAxes { get; } = new Axis[]
        {
            new Axis
            {
                Name = "價格 (USD)",
                NamePaint = new SolidColorPaint(SKColors.White),
                LabelsPaint = new SolidColorPaint(SKColors.LightGray),
                TextSize = 10,
                SeparatorsPaint = new SolidColorPaint(SKColors.Gray) { StrokeThickness = 1 },
                ShowSeparatorLines = true
            }
        };

        /// <summary>
        /// 價格變化的顏色（用於綁定UI）
        /// </summary>
        public string PriceChangeColor => PriceChange >= 0 ? "#4CAF50" : "#F44336";

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
            var lineSeries = new LineSeries<ObservablePoint>
            {
                Values = new ObservableCollection<ObservablePoint>(),
                Name = "股價走勢",
                GeometrySize = 4,
                GeometryStroke = new SolidColorPaint(SKColors.DeepSkyBlue) { StrokeThickness = 2 },
                GeometryFill = new SolidColorPaint(SKColors.White),
                Stroke = new SolidColorPaint(SKColors.DeepSkyBlue) { StrokeThickness = 2 },
                Fill = new LinearGradientPaint(
                    new[] { SKColors.DeepSkyBlue.WithAlpha(100), SKColors.Transparent },
                    new SKPoint(0, 0),
                    new SKPoint(0, 1)
                )
            };

            Series.Add(lineSeries);
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

                // 按時間排序並添加到圖表
                var sortedPrices = quote.HistoricalPrices
                    .OrderBy(p => p.Timestamp)
                    .ToList();

                for (int i = 0; i < sortedPrices.Count; i++)
                {
                    var price = sortedPrices[i];
                    values.Add(new ObservablePoint(i, (double)price.Price));
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
