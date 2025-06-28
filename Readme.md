# 📈 NetStockTool - WPF 迷你股票走勢圖工具

一個現代化的 Windows 桌面股票監控小工具，使用 WPF 和 LiveCharts 技術製作，提供即時股價顯示和互動式走勢圖表。

![License](https://img.shields.io/badge/license-MIT-blue.svg)
![.NET](https://img.shields.io/badge/.NET-9.0-purple.svg)
![WPF](https://img.shields.io/badge/WPF-Windows-blue.svg)
![Build Status](https://img.shields.io/badge/build-passing-brightgreen.svg)

## 🌟 主要特色

### 🎯 核心功能
- **📊 即時股價顯示** - 顯示股票當前價格、漲跌幅和變化百分比
- **📈 互動式走勢圖** - 使用 LiveCharts 繪製高品質股價走勢圖表
- **🖥️ 桌面小工具** - 透明背景浮動窗口，永遠置頂顯示
- **🖱️ 拖拽移動** - 點擊並拖拽窗口到任意位置
- **⏰ 實時時鐘** - 顯示即時更新的當前時間
- **🔄 自動更新** - 背景服務定期更新股票資料

### 🎨 UI 設計特色
- **🌃 現代化黑色主題** - 使用深色主題與漸層背景
- **🎯 智慧顏色指示**：
  - 🟢 綠色：股價上漲
  - 🔴 紅色：股價下跌
  - ⚪ 灰色：無變化
- **🔄 載入動畫** - 旋轉載入指示器
- **📱 圓角設計** - 8px 圓角現代化邊框
- **✨ 陰影效果** - 立體視覺效果

### 📊 股價資訊顯示
- **🏷️ 股票代號** - 顯示股票代碼（如 TSLA）
- **🏢 公司名稱** - 顯示公司全名
- **💰 當前價格** - 即時股價（美元顯示）
- **📈 漲跌金額** - 絕對變化值（+/-）
- **📉 漲跌百分比** - 相對變化百分比
- **🕐 更新時間** - 股票資料最後更新時間
- **⏰ 當前時間** - 實時顯示當前時間
- **📊 30天走勢圖** - 顯示最近30天價格走勢

## 🏗️ 技術架構

### 🖼️ 前端架構 (MiniStockView)
- **🖥️ WPF** - 使用 .NET 9.0 和 WPF 框架
- **🔗 MVVM 模式** - 使用 CommunityToolkit.Mvvm 實現
- **📊 LiveCharts** - 高效能股價走勢圖表顯示
- **💉 依賴注入** - Microsoft.Extensions.DependencyInjection

### ⚙️ 核心業務 (MiniStockWidget.Core)
- **🏛️ 服務層設計** - 乾淨的業務邏輯分離
- **🗄️ 快取機制** - 智慧記憶體快取系統（5分鐘快取）
- **🔄 背景服務** - 定期自動更新股票資料（30秒間隔）
- **🌐 HTTP 客戶端** - 支援重試機制的 API 呼叫

## 📁 專案結構

```
NetStockTool/
├── 📁 MiniStockView/                    # WPF 前端應用程式
│   ├── 📁 ViewModels/                   # MVVM 視圖模型
│   │   └── MainViewModel.cs            # 主視圖模型
│   ├── 📁 Converters/                   # 值轉換器
│   │   └── ValueConverters.cs          # 顏色和格式轉換器
│   ├── 📄 MainWindow.xaml              # 主視窗 UI 設計
│   ├── 📄 MainWindow.xaml.cs           # 主視窗程式碼
│   ├── 📄 App.xaml                     # 應用程式資源
│   ├── 📄 App.xaml.cs                  # 應用程式入口點
│   └── 📄 MiniStockView.csproj         # 專案配置檔
├── 📁 src/MiniStockWidget.Core/         # 核心業務邏輯層
│   ├── 📁 Models/                       # 資料模型
│   │   ├── StockQuote.cs               # 股票報價模型
│   │   └── HistoricalPrice.cs          # 歷史價格模型
│   ├── 📁 Services/                     # 服務層
│   │   ├── IQuoteService.cs            # 股價服務介面
│   │   ├── YahooQuoteService.cs        # Yahoo Finance 實作
│   │   └── QuoteBackgroundService.cs   # 背景更新服務
│   ├── 📁 Cache/                        # 快取層
│   │   └── QuoteCache.cs               # 報價快取服務
│   └── 📄 MiniStockWidget.Core.csproj  # 核心專案配置
├── 📁 tests/MiniStockWidget.Tests/      # 單元測試
│   ├── 📄 UnitTest1.cs                 # 測試檔案
│   └── 📄 MiniStockWidget.Tests.csproj # 測試專案配置
├── 📄 MiniStockWidget.sln              # Visual Studio 解決方案
├── 📄 run.bat                          # 快速執行腳本
└── 📄 README.md                        # 專案說明文件
```

## 🚀 快速開始

### 📋 環境需求

- **🖥️ 作業系統**: Windows 10/11
- **⚡ .NET**: .NET 9.0 或更高版本
- **🛠️ 開發工具**: Visual Studio 2022 或 VS Code
- **💾 記憶體**: 建議 4GB 以上
- **🖥️ 螢幕解析度**: 建議 1920x1080 以上

### 📥 安裝與執行

#### 方法一：快速執行（推薦）

```bash
# 1. 克隆專案
git clone https://github.com/linyos/NetStockTool.git

# 2. 進入專案目錄
cd NetStockTool

# 3. 執行快速啟動腳本
run.bat
```

#### 方法二：手動建置

```bash
# 1. 還原 NuGet 套件
dotnet restore

# 2. 建置整個解決方案
dotnet build --configuration Release

# 3. 執行應用程式
cd MiniStockView
dotnet run --configuration Release

# 4. 執行單元測試（可選）
dotnet test
```

### 🖱️ 使用方式

1. **🚀 啟動應用程式**: 應用程式會自動顯示在桌面右上角
2. **👆 拖拽移動**: 左鍵點擊並拖拽窗口到合適位置
3. **👁️ 檢視資料**: 查看即時股價和 30 天走勢圖
4. **🖱️ 右鍵操作**: 右鍵點擊窗口顯示操作選單
5. **🔄 刷新資料**: 透過右鍵選單手動刷新或等待自動更新

## 📦 相依套件

### MiniStockView (UI 層)
- **LiveChartsCore.SkiaSharpView.WPF** `2.0.0-rc3` - 高效能圖表繪製
- **CommunityToolkit.Mvvm** `8.2.2` - MVVM 框架支援
- **Microsoft.Extensions.Hosting** `9.0.6` - 依賴注入和背景服務
- **Microsoft.Extensions.DependencyInjection** `9.0.6` - 依賴注入容器
- **Microsoft.Extensions.Http** `9.0.6` - HTTP 客戶端服務

### MiniStockWidget.Core (核心層)
- **Microsoft.Extensions.Logging** `9.0.6` - 日誌記錄服務
- **Microsoft.Extensions.Http** `9.0.6` - HTTP 客戶端工廠
- **Microsoft.Extensions.Hosting** `9.0.6` - 背景服務支援

### MiniStockWidget.Tests (測試層)
- **xunit** `2.5.3` - 單元測試框架
- **FluentAssertions** `8.4.0` - 流暢的斷言語法
- **Microsoft.NET.Test.Sdk** `17.8.0` - 測試 SDK

## ⚙️ 配置選項

### 🔧 自訂股票代號

在 `MainViewModel.cs` 中修改：

```csharp
private string _symbol = "AAPL"; // 改成您想要監視的股票代號
```

### 📐 調整視窗大小

在 `MainWindow.xaml` 中修改：

```xml
Height="280"    <!-- 視窗高度 -->
Width="380"     <!-- 視窗寬度 -->
```

### ⏱️ 設定更新頻率

在 `QuoteBackgroundService.cs` 中調整：

```csharp
private TimeSpan _updateInterval = TimeSpan.FromSeconds(30); // 30秒更新一次
```

### 🎨 自訂主題顏色

在 `App.xaml` 或 `MainWindow.xaml` 中修改顏色設定。

## 🔄 背景服務功能

### 🤖 智慧背景更新
- **⏰ 自動更新**: 每 30 秒自動獲取最新股價
- **🔄 錯誤重試**: 內建指數退避重試機制（最多 3 次）
- **🗄️ 快取策略**: 5 分鐘快取避免頻繁 API 呼叫
- **🧵 UI 同步**: 自動在 UI 線程中更新介面
- **📊 記憶體管理**: 智慧清理過期快取資料

### 📈 圖表功能

#### LiveCharts 整合特色
- **⚡ 即時更新**: 股價變化時圖表即時重繪
- **🎬 流暢動畫**: 平滑的資料變化動畫效果
- **🖱️ 互動提示**: 滑鼠懸停顯示詳細資訊
- **📏 自適應軸**: 自動調整 X/Y 軸範圍

#### 視覺樣式
- **🌊 漸層填充**: 美觀的藍色漸層填充效果
- **📍 資料點標記**: 清晰的資料點標示
- **🔲 格線顯示**: 淺色格線輔助讀取數值
- **🌃 深色主題**: 適合小工具的深色圖表主題

## 🧪 測試

### 執行所有測試

```bash
dotnet test
```

### 執行特定測試

```bash
dotnet test --filter "TestMethodName"
```

### 測試覆蓋率

```bash
dotnet test --collect:"XPlat Code Coverage"
```

## 🛠️ 開發指南

### 🆕 新增股票數據源

1. 實作 `IQuoteService` 介面
2. 在 `App.xaml.cs` 中註冊新服務
3. 實作對應的 API 解析邏輯

### 🎨 擴展 UI 功能

1. 在 `MainViewModel.cs` 中新增屬性
2. 在 `MainWindow.xaml` 中新增 UI 元件
3. 使用資料綁定連接 ViewModel 和 View

### 🔄 新增值轉換器

在 `ValueConverters.cs` 中實作 `IValueConverter` 介面。

### 📊 自訂圖表樣式

修改 `MainViewModel.cs` 中的 `XAxes` 和 `YAxes` 配置。

## 🚧 已知限制與注意事項

- **🔌 模擬資料**: 目前使用模擬資料（需要整合真實 API）
- **📱 單股顯示**: 僅支援單一股票顯示
- **🌐 網路相依**: 網路連線問題可能影響資料更新
- **🖥️ Windows 限定**: 僅支援 Windows 作業系統
- **⚡ 效能考量**: 長時間運行建議定期重啟應用程式

## 🔮 未來規劃

### 近期計劃
- [ ] 🔌 整合真實的 Yahoo Finance API
- [ ] 📊 支援多檔股票監視列表
- [ ] 🔔 新增股價提醒功能
- [ ] 📈 支援更多技術指標（MA、RSI、MACD）
- [ ] 🎨 自訂主題顏色選擇器

### 中期目標
- [ ] 💰 支援加密貨幣價格監控
- [ ] 📤 匯出歷史資料功能
- [ ] 📱 支援多螢幕顯示
- [ ] 🔍 股票搜尋功能
- [ ] 📋 投資組合管理

### 長期願景
- [ ] 🌐 Web 版本開發
- [ ] 📱 移動應用程式
- [ ] 🤖 AI 投資建議
- [ ] 📊 更多圖表類型支援
- [ ] 🔗 社群分享功能


### 🐛 回報問題
1. 請先檢查 [Issues](https://github.com/linyos/NetStockTool/issues) 是否已有相同問題
2. 提供詳細的問題描述和重現步驟
3. 附上螢幕截圖或錯誤訊息

### 💡 提出建議
1. 在 [Issues](https://github.com/linyos/NetStockTool/issues) 中建立功能請求
2. 詳細描述建議的功能和使用場景
3. 如果可能，提供設計稿或原型

### 🔧 程式碼貢獻
1. Fork 此專案
2. 建立功能分支 (`git checkout -b feature/AmazingFeature`)
3. 提交變更 (`git commit -m 'feat: Add some AmazingFeature'`)
4. 推送到分支 (`git push origin feature/AmazingFeature`)
5. 開啟 Pull Request

### 📝 提交訊息規範
請遵循 [Conventional Commits](https://www.conventionalcommits.org/) 規範：

- `feat:` 新功能
- `fix:` 修復錯誤
- `docs:` 文件更新
- `style:` 程式碼格式修改
- `refactor:` 程式碼重構
- `test:` 測試相關
- `chore:` 建置過程或輔助工具的變動

## 📄 授權條款

此專案採用 MIT 授權條款。詳細內容請參閱 [LICENSE](LICENSE) 檔案。




</div>