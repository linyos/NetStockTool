using System.Configuration;
using System.Data;
using System.Text;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MiniStockWidget.Core.Services;
using MiniStockWidget.Core.Cache;
using MiniStockView.ViewModels;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using SkiaSharp;

namespace MiniStockView
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IHost? _host;

        protected override void OnStartup(StartupEventArgs e)
        {
            // 設置控制台編碼為UTF-8，解決中文顯示問題
            try
            {
                Console.OutputEncoding = Encoding.UTF8;
                Console.InputEncoding = Encoding.UTF8;
            }
            catch
            {
                // 如果設置失敗，忽略錯誤繼續執行
            }

            // 配置 LiveChartsCore 全局設定，使用英文字型
            try
            {
                // 設定 SkiaSharp 的字型管理器
                var fontManager = SKFontManager.Default;
                var englishFontFamilies = new[] { "Segoe UI", "Arial", "Calibri", "Verdana", "Tahoma" };

                // 嘗試找到英文字型
                SKTypeface? englishTypeface = null;
                foreach (var fontFamily in englishFontFamilies)
                {
                    englishTypeface = fontManager.MatchFamily(fontFamily);
                    if (englishTypeface != null && !string.IsNullOrEmpty(englishTypeface.FamilyName))
                    {
                        break;
                    }
                }

                LiveCharts.Configure(config =>
                {
                    config
                        .AddSkiaSharp() // 添加 SkiaSharp 支援
                        .AddDefaultMappers()
                        .AddLightTheme(); // 使用淺色主題

                    // 設定英文字型為全局字型
                    if (englishTypeface != null)
                    {
                        config.HasGlobalSKTypeface(englishTypeface);
                    }
                });
            }
            catch
            {
                // 如果字型設定失敗，使用預設配置
                LiveCharts.Configure(config =>
                {
                    config
                        .AddSkiaSharp()
                        .AddDefaultMappers()
                        .AddLightTheme();
                });
            }

            base.OnStartup(e);

            // 建立和配置Host
            _host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    // 註冊核心服務
                    // services.AddHttpClient("Yahoo", client =>
                    // {
                    //     client.DefaultRequestHeaders.Add("User-Agent",
                    //         "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
                    // });
                    // services.AddSingleton<IQuoteService, YahooQuoteService>();


                    services.AddHttpClient(); // 這行是必要的，提供 IHttpClientFactory
                    services.AddHttpClient("TWSE", client =>
                    {
                        client.DefaultRequestHeaders.Add("User-Agent",
                            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");
                        client.DefaultRequestHeaders.Add("Accept-Charset", "UTF-8");
                        client.DefaultRequestHeaders.Add("Accept-Language", "zh-TW,zh;q=0.9,en;q=0.8");
                        client.Timeout = TimeSpan.FromSeconds(30); // 設定超時時間
                    });
                    services.AddScoped<IQuoteService, TwseQuoteService>(); // 使用台灣證交所服務


                    services.AddSingleton<QuoteCache>();
                    services.AddSingleton<QuoteBackgroundService>();

                    // 註冊ViewModel
                    services.AddTransient<MainViewModel>();

                    // 註冊MainWindow
                    services.AddTransient<MainWindow>();
                })
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                    logging.SetMinimumLevel(LogLevel.Information);
                })
                .Build();

            // 啟動Host
            _host.Start();

            // 創建並顯示主視窗
            var mainWindow = _host.Services.GetRequiredService<MainWindow>();
            var viewModel = _host.Services.GetRequiredService<MainViewModel>();

            mainWindow.SetViewModel(viewModel);
            mainWindow.Show();

            // 啟動背景服務
            var backgroundService = _host.Services.GetRequiredService<QuoteBackgroundService>();
            backgroundService.QuotesUpdated += (sender, quotes) =>
            {
                // 在UI線程中更新ViewModel
                Dispatcher.Invoke(() =>
                {
                    if (quotes?.Count > 0)
                    {
                        var quote = quotes.FirstOrDefault(q => q.Symbol == viewModel.Symbol);
                        if (quote != null)
                        {
                            viewModel.CompanyName = quote.CompanyName;
                            viewModel.CurrentPrice = quote.Price;
                            viewModel.PriceChange = quote.Change;
                            viewModel.ChangePercent = quote.ChangePercent;
                            viewModel.LastUpdate = quote.UpdateTime;
                        }
                    }
                });
            };

            MainWindow = mainWindow;
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _host?.Dispose();
            base.OnExit(e);
        }
    }
}
