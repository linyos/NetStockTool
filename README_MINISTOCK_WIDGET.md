# MiniStock Widget - WPF 迷你股票走勢圖應用程式

## 📊 應用程式概覽

這是一個使用 WPF 和 LiveCharts 製作的迷你股票走勢圖桌面小工具，提供即時股價顯示和走勢圖表功能。

![MiniStock Widget Preview](preview.png)

## ✨ 主要特色

### 🎯 核心功能
- **即時股價顯示**: 顯示股票的當前價格、漲跌幅和變化百分比
- **互動式走勢圖**: 使用 LiveCharts 繪製的高品質股價走勢圖
- **桌面小工具**: 透明背景的浮動窗口，可置頂顯示
- **拖拽移動**: 點擊並拖拽窗口到任意位置
- **右鍵選單**: 提供刷新資料和關閉功能

### 🎨 UI 設計特色
- **現代化黑色主題**: 使用深色主題與漸層背景
- **智慧顏色指示**: 
  - 🟢 綠色：股價上漲
  - 🔴 紅色：股價下跌
  - ⚪ 灰色：無變化
- **圓角設計**: 8px 圓角的現代化邊框
- **陰影效果**: 立體視覺效果
- **載入動畫**: 旋轉載入指示器

### 📊 股價資訊顯示
- **股票代號**: 顯示股票代碼（如 TSLA）
- **公司名稱**: 顯示公司全名
- **當前價格**: 即時股價 (以美元顯示)
- **漲跌金額**: 絕對變化值 (+/-)
- **漲跌百分比**: 相對變化百分比
- **更新時間**: 最後更新的時間戳
- **30天走勢圖**: 顯示最近30天的價格走勢

## 🔧 技術架構

### 前端框架 (MiniStockView)
- **WPF**: 使用 .NET 9 和 WPF 框架
- **MVVM 模式**: 使用 CommunityToolkit.Mvvm 實現
- **LiveCharts**: 高效能的股價走勢圖表顯示
- **依賴注入**: 使用 Microsoft.Extensions.DependencyInjection

### 核心業務 (MiniStockWidget.Core)
- **服務層設計**: 乾淨的業務邏輯分離
- **快取機制**: 智慧的記憶體快取系統
- **背景服務**: 定期自動更新股價資料
- **HTTP 客戶端**: 支援重試機制的API呼叫

## 🚀 快速開始

### 環境需求
- .NET 9.0 或更高版本
- Windows 作業系統 (Windows 10/11)
- Visual Studio 2022 或 VS Code

### 安裝與執行

```bash
# 1. 克隆專案
git clone [your-repo-url]

# 2. 進入專案目錄
cd NetStockTool

# 3. 還原套件
dotnet restore

# 4. 建置專案
dotnet build

# 5. 執行應用程式
cd MiniStockView
dotnet run
```

### 使用方式

1. **啟動程式**: 應用程式會自動顯示在桌面右上角
2. **拖拽移動**: 左鍵點擊並拖拽窗口到合適位置
3. **檢視資料**: 查看即時股價和走勢圖
4. **右鍵操作**: 右鍵點擊窗口顯示操作選單
5. **刷新資料**: 透過右鍵選單手動刷新或等待自動更新

## 📁 專案結構

```
NetStockTool/
├── MiniStockView/                    # WPF 前端應用程式
│   ├── ViewModels/                   # MVVM 視圖模型
│   │   └── MainViewModel.cs         # 主視圖模型
│   ├── Converters/                   # 值轉換器
│   │   └── ValueConverters.cs       # 顏色和格式轉換器
│   ├── MainWindow.xaml              # 主視窗 UI
│   ├── MainWindow.xaml.cs           # 主視窗程式碼
│   ├── App.xaml                     # 應用程式資源
│   ├── App.xaml.cs                  # 應用程式入口點
│   └── MiniStockView.csproj         # 專案設定檔
├── src/MiniStockWidget.Core/         # 核心業務邏輯
│   ├── Models/                       # 資料模型
│   │   ├── StockQuote.cs            # 股票報價模型
│   │   └── HistoricalPrice.cs       # 歷史價格模型
│   ├── Services/                     # 服務層
│   │   ├── IQuoteService.cs         # 股價服務介面
│   │   ├── YahooQuoteService.cs     # Yahoo Finance 實作
│   │   └── QuoteBackgroundService.cs # 背景更新服務
│   ├── Cache/                        # 快取層
│   │   └── QuoteCache.cs            # 報價快取服務
│   └── MiniStockWidget.Core.csproj  # 核心專案設定
└── tests/MiniStockWidget.Tests/      # 單元測試
    ├── UnitTest1.cs                 # 測試檔案
    └── MiniStockWidget.Tests.csproj # 測試專案設定
```

## ⚙️ 自訂設定

### 修改監視的股票代號
在 `MainViewModel.cs` 中修改預設股票：
```csharp
private string _symbol = "AAPL"; // 改成您想要監視的股票代號
```

### 調整視窗大小
在 `MainWindow.xaml` 中修改視窗尺寸：
```xml
Height="280"
Width="380"
```

### 設定更新頻率
在 `QuoteBackgroundService.cs` 中調整自動更新間隔：
```csharp
private TimeSpan _updateInterval = TimeSpan.FromSeconds(30); // 30秒更新一次
```

### 自訂視覺主題
在 `App.xaml` 中修改全域樣式，或在 `MainWindow.xaml` 中調整顏色設定。

## 🔄 背景服務

應用程式包含一個智慧的背景更新服務：

- **自動更新**: 每30秒自動獲取最新股價
- **錯誤重試**: 內建指數退避重試機制
- **快取策略**: 5分鐘快取避免頻繁API呼叫
- **UI同步**: 自動在UI線程中更新介面

## 📈 圖表功能

### LiveCharts 整合
- **即時更新**: 股價變化時圖表即時重繪
- **流暢動畫**: 平滑的資料變化動畫
- **互動提示**: 滑鼠懸停顯示詳細資訊
- **自適應軸**: 自動調整X/Y軸範圍

### 圖表樣式
- **漸層填充**: 美觀的藍色漸層填充
- **資料點標記**: 清晰的資料點顯示
- **格線**: 淺色格線幫助讀取數值
- **深色主題**: 適合小工具的深色圖表主題

## 🛠️ 開發指南

### 新增股票數據源
1. 實作 `IQuoteService` 介面
2. 在 `App.xaml.cs` 中註冊新服務
3. 實作對應的API解析邏輯

### 擴展UI功能
1. 在 `MainViewModel.cs` 中新增屬性
2. 在 `MainWindow.xaml` 中新增UI元件
3. 使用資料綁定連接ViewModel和View

### 新增值轉換器
在 `ValueConverters.cs` 中實作 `IValueConverter` 介面。

## 🚧 已知限制

- 目前使用模擬資料（需要實際API整合）
- 僅支援單一股票顯示
- 網路連線問題可能影響資料更新

## 🔮 未來規劃

- [ ] 整合真實的Yahoo Finance API
- [ ] 支援多檔股票監視列表
- [ ] 新增股價提醒功能
- [ ] 支援更多技術指標
- [ ] 自訂主題顏色
- [ ] 匯出歷史資料功能
- [ ] 支援加密貨幣價格

## 📄 授權

此專案採用 MIT 授權條款。

## 🤝 貢獻

歡迎提交 Pull Request 或建立 Issue 來改進這個專案！

---

**注意**: 本應用程式僅供學習和個人使用，不構成投資建議。股市有風險，投資需謹慎。
