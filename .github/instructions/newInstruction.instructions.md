---
applyTo: '**/*.cs'
---
# .NET 和 C# 程式碼規範

套用 [一般程式碼準則](./general-coding.instructions.md) 到所有程式碼。

## C# 語言特定準則

- 使用 var 關鍵字當型別明顯時
- 優先使用表達式主體成員（expression-bodied members）
- 使用字串插值而非 string.Format
- 實作 IDisposable 介面處理非託管資源
- 使用 nullable reference types

## ASP.NET Core 準則

- 控制器繼承自 ControllerBase
- 使用 [ApiController] 屬性
- 實作適當的模型繫結和驗證
- 使用 Minimal APIs 於簡單端點
- 配置適當的 CORS 政策

## Entity Framework Core 準則

- DbContext 使用依賴注入
- 使用 Fluent API 進行複雜設定
- 實作適當的索引策略
- 使用 migrations 管理資料庫結構變更