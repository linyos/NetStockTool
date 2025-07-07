---
applyTo: '**/*.cs'
---
# .NET 專案開發規範

## C# 命名慣例

- 類別、介面、方法使用 PascalCase（例：UserService、IRepository）
- 變數、欄位使用 camelCase（例：userName、userId）
- 私有欄位使用底線開頭（例：\_context、\_logger）
- 常數使用 PascalCase（例：MaxRetryCount）

## .NET 程式碼品質要求

- 所有公開方法都必須包含 XML 文件註解
- 使用 async/await 進行異步操作，方法名以 Async 結尾
- 優先使用 LINQ 進行集合操作
- 使用 using 陳述式進行資源管理

## ASP.NET Core 最佳實務

- 控制器方法要有適當的 HTTP 動詞屬性
- 使用 ActionResult<T> 作為返回類型
- 實作適當的模型驗證
- 使用依賴注入進行服務註冊

## Entity Framework 規範

- 使用 IQueryable 進行資料庫查詢
- 實作 Repository 或 Unit of Work 模式
- 使用 Include() 進行關聯資料載入
- 避免 N+1 查詢問題

## 錯誤處理

- 使用 try-catch-finally 區塊處理例外
- 記錄詳細的錯誤資訊到 ILogger
- 回傳適當的 HTTP 狀態碼
- 使用自訂例外類別