# MiniStock Widget - 股價小工具

## 功能特色

這是一個使用 WPF 和 LiveCharts 製作的股價小工具，具有以下特色：

### 🎯 核心功能
- **即時股價顯示**: 顯示股票的當前價格、漲跌幅和變化百分比
- **走勢圖表**: 使用 LiveCharts 繪製股價走勢圖
- **桌面小工具**: 透明背景的浮動窗口，可固定在桌面上方
- **拖拽移動**: 點擊並拖拽窗口到任意位置
- **右鍵選單**: 提供刷新資料和關閉功能

### 🎨 UI 設計特色
- **現代化界面**: 使用深色主題與漸層背景
- **顏色指示**: 
  - 🟢 綠色：股價上漲
  - 🔴 紅色：股價下跌
  - ⚪ 灰色：無變化
- **圓角邊框**: 8px 圓角的現代化設計
- **透明效果**: 支援透明背景和視覺效果

### 📊 股價資訊顯示
- **股票代號**: 顯示股票代碼（如 TSLA）
- **公司名稱**: 顯示公司全名
- **當前價格**: 實時股價
- **漲跌金額**: 絕對變化值
- **漲跌百分比**: 相對變化百分比
- **更新時間**: 最後更新的時間戳

### 🔧 技術架構
- **WPF**: 使用 .NET 8 和 WPF 框架
- **MVVM 模式**: 使用 CommunityToolkit.Mvvm 實現
- **LiveCharts**: 股價走勢圖表顯示
- **依賴注入**: 使用 Microsoft.Extensions.DependencyInjection
- **背景服務**: 定期更新股價資料

## 使用方式

### 基本操作
1. **啟動程式**: 運行 `dotnet run` 或執行編譯後的程式
2. **拖拽移動**: 左鍵點擊並拖拽窗口到合適位置
3. **右鍵選單**: 右鍵點擊窗口顯示操作選單
4. **刷新資料**: 透過右鍵選單或定時自動更新

### 互動功能
- **拖拽**: 可自由移動窗口位置
- **置頂**: 窗口永遠保持在最前方
- **右鍵選單**: 
  - 刷新資料
  - 關閉程式

## 建置與執行

### 環境需求
- .NET 8.0 或更高版本
- Windows 作業系統
- Visual Studio 2022 或 VS Code

### 建置步驟
```bash
# 克隆專案
git clone [your-repo-url]

# 進入專案目錄
cd NetStockTool

# 還原套件
dotnet restore

# 建置專案
dotnet build

# 執行程式
cd src/MiniStockWidget.App
dotnet run
```

### 套件依賴
- `LiveChartsCore.SkiaSharpView.WPF`: 圖表繪製
- `CommunityToolkit.Mvvm`: MVVM 模式支援
- `Microsoft.Extensions.Hosting`: 依賴注入和背景服務
- `Microsoft.Extensions.Http`: HTTP 客戶端服務

## 程式碼結構

```
src/
├── MiniStockWidget.App/          # WPF 應用程式
│   ├── Views/                    # 視圖
│   │   ├── MainWindow.xaml       # 主視窗 XAML
│   │   └── MainWindow.xaml.cs    # 主視窗程式碼
│   ├── ViewModels/               # 視圖模型
│   │   └── MainViewModel.cs      # 主視圖模型
│   ├── Converters/               # 值轉換器
│   │   └── ColorConverter.cs     # 顏色轉換器
│   └── App.xaml(.cs)            # 應用程式入口
├── MiniStockWidget.Core/         # 核心業務邏輯
│   ├── Models/                   # 資料模型
│   │   ├── StockQuote.cs        # 股票報價模型
│   │   └── HistoricalPrice.cs   # 歷史價格模型
│   └── Services/                 # 服務層
│       ├── IQuoteService.cs     # 股價服務介面
│       ├── YahooQuoteService.cs # Yahoo 股價服務
│       └── QuoteBackgroundService.cs # 背景更新服務
```

## 自訂設定

### 修改股票代號
在 `MainViewModel.cs` 中修改：
```csharp
private string _symbol = "TSLA"; // 改成您想要的股票代號
```

### 調整視窗大小
在 `MainWindow.xaml` 中修改：
```xml
Height="180"
Width="320"
```

### 更新頻率
在 `QuoteBackgroundService.cs` 中調整定時器間隔。

## 未來計劃

- [ ] 支援多檔股票同時顯示
- [ ] 新增股票搜尋功能
- [ ] 支援更多數據源（如 Alpha Vantage）
- [ ] 新增技術指標顯示
- [ ] 支援自訂主題色彩
- [ ] 新增股價提醒功能
- [ ] 支援匯出資料功能

## 貢獻

歡迎提交 Pull Request 或建立 Issue 來改進這個專案！

## 授權

此專案採用 MIT 授權條款。
