---
mode: agent
title: MiniStockWidget Development Prompt
---
Expected output and any relevant constraints for this task.

# 📈 MiniStockWidget – 開發 Prompt

> 一份 **單檔 Markdown**，定義此桌面小工具的願景、技術選型、資料夾與命名規範、開發指令與驗收條件。  
> 所有成員請務必熟讀並保持同步。

---

## 0. TL;DR
用 **.NET 8 + WPF + MVVM** 打造可「常駐桌面、即時更新」的迷你股票走勢圖元件。  
核心模組抽離至 `MiniStockWidget.Core`，可於日後移植至 WinUI 3 / .NET MAUI / Avalonia。  

---

## 1. 功能範圍 (MVP)

| 模組            | 主要內容 | Done 定義 |
|-----------------|----------|-----------|
| Quote Service   | • 透過 Yahoo Finance API 抓取最新報價<br>• 支援複數 Symbol, 可自訂更新頻率 | ① 10 檔台股/美股都能回傳 <br>② Request 失敗須自動 Retry（最多 3 次） |
| Background Job  | • `IHostedService` 週期性排程 <br>• 不阻塞 UI Thread | ① 在 Task Manager 可見僅 1% CPU <br>② UI 與排程皆可關閉 |
| Mini Chart View | • 折線圖（LiveCharts2）<br>• 最新價、漲跌、% 顯示 | ① 切 Symbol 圖面 < 150ms 更新 <br>② 暗黑 / 淺色自動配色 |
| Window Shell    | • 無邊框、可拖曳<br>• Top-most<br>• 右擊彈出選單 | ① Win + ←/→ snap 無異常 |
| 自動啟動         | • 安裝後可選「開機啟動」 | ① Windows 登錄檔出現對應 Key |

> 🔜 待 M1 穩定後再納入：小時 / 日 K、技術指標、匯率、TrayIcon Preview。  

---

## 2. 技術選型

| 類別 | 選用 | 理由 |
|------|------|------|
| UI   | WPF (.NET 8) + CommunityToolkit.Mvvm | 你本人熟悉；LiveCharts2 相容佳 |
| DI   | `Microsoft.Extensions.DependencyInjection` | 與 ASP.NET Core 同生態 |
| HTTP | `HttpClientFactory` + `System.Text.Json` | 原生、免第三方 |
| 圖表 | LiveCharts2 | 輕量、支援 WPF 與 MAUI |
| 測試 | xUnit + FluentAssertions | 社群主流 |
| 打包 | PublishSingleFile + Trimmed / MSIX | 取決於是否要自動更新 |

---


## 2.1 UI 架構與建立步驟（補充）

### 建立 WPF UI 專案與 MVVM 架構

1. **建立 WPF 專案**
   ```shell
   dotnet new wpf -n MiniStockWidget.App -f net8.0
   dotnet sln add src/MiniStockWidget.App/MiniStockWidget.App.csproj
   ```
2. **參考 Core 專案**
   ```shell
   dotnet add src/MiniStockWidget.App/MiniStockWidget.App.csproj reference src/MiniStockWidget.Core/MiniStockWidget.Core.csproj
   ```
3. **安裝 MVVM Toolkit 與 LiveCharts2**
   ```shell
   dotnet add src/MiniStockWidget.App/MiniStockWidget.App.csproj package CommunityToolkit.Mvvm
   dotnet add src/MiniStockWidget.App/MiniStockWidget.App.csproj package LiveChartsCore.SkiaSharpView.WPF
   ```
4. **建立 ViewModel 與 View**
   - 在 `ViewModels/` 建立 `MainViewModel.cs`，繼承 `ObservableObject`。
   - 在 `Views/` 建立 `MainWindow.xaml`，用 LiveCharts2 畫折線圖，資料繫結到 ViewModel。
5. **DI 註冊與啟動**
   - 在 `App.xaml.cs` 依下方範例註冊 DI 與 HostedService。

---

## 3. Solution 結構

```
MiniStockWidget.sln
│
├─ src/
│   ├─ MiniStockWidget.App      (WPF, UI Layer)
│   │     ├─ Views/
│   │     ├─ ViewModels/
│   │     ├─ Converters/
│   │     └─ Resources/
│   │
│   └─ MiniStockWidget.Core     (Class Library, 無 UI)
│         ├─ Models/
│         ├─ Services/          (IQuoteService, YahooQuoteService,…)
│         ├─ Cache/             (InMemory, optional Sqlite)
│         └─ Utils/
│
└─ tests/
└─ MiniStockWidget.Tests    (xUnit)



```

命名規範：PascalCase 專案名稱；資料夾名稱 = 模式 / 責任；`I*Service` 為介面。  

---

## 4. 關鍵範例片段

### 4.1 DI 註冊
```csharp
// App.xaml.cs
Host = Host.CreateDefaultBuilder()
    .ConfigureServices((ctx, svcs) =>
    {
        svcs.AddHttpClient("Yahoo", c =>
            c.BaseAddress = new("https://query1.finance.yahoo.com/"));
        svcs.AddTransient<IQuoteService, YahooQuoteService>();
        svcs.AddSingleton<MainViewModel>();
        svcs.AddHostedService<QuoteBackgroundService>();
    })
    .Build();

### 4.2 LiveCharts2 Mini Line
```csharp
<!-- StockMiniView.xaml -->
<lvc:CartesianChart Series="{Binding Series}"
                    LegendPosition="None" ZoomMode="None"
                    Width="120" Height="40" Background="Transparent"/>

```

## 5. 開發腳本
# 5.1. 還原與編譯
dotnet restore
dotnet build -c Release

# 5.2. 執行 (附加熱重載)
dotnet watch --project src/MiniStockWidget.App run

# 5.3. 打包單檔
dotnet publish src/MiniStockWidget.App -c Release -r win-x64 ^
-p:PublishSingleFile=true -p:PublishTrimmed=true --self-contained


6. Coding Guideline

Async All The Way：Service 層不允許 .Result / .Wait()。
ViewModel <-> View 僅透過 Binding；不得於 CodeBehind 操作狀態。
每支 PR 至少 1 個單元測試，覆蓋新邏輯的 Happy Path + 失敗重試。
UI 調色寫在 Resources/Colors.xaml，避免硬編碼。
例外狀況統一使用 ILogger 記錄，UI 層只顯示友善訊息。