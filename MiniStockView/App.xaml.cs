using System.Configuration;
using System.Data;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MiniStockWidget.Core.Services;
using MiniStockWidget.Core.Cache;
using MiniStockView.ViewModels;

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
            base.OnStartup(e);

            // 建立和配置Host
            _host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    // 註冊核心服務
                    services.AddHttpClient("Yahoo", client =>
                    {
                        client.DefaultRequestHeaders.Add("User-Agent",
                            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
                    });

                    services.AddSingleton<IQuoteService, YahooQuoteService>();
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
