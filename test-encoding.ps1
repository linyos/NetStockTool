# 測試 PowerShell 編碼設置
$OutputEncoding = [console]::InputEncoding = [console]::OutputEncoding = New-Object System.Text.UTF8Encoding

# 測試中文字符輸出
Write-Host "測試中文編碼：元大台灣50" -ForegroundColor Green
Write-Host "更新時間：$(Get-Date -Format 'HH:mm:ss')" -ForegroundColor Yellow

# 運行應用程式
cd "d:\Coding\NetStock\NetStockTool"
dotnet run --project MiniStockView
