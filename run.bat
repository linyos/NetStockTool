@echo off
echo =====================================================
echo  MiniStock Widget - 股票走勢圖應用程式
echo =====================================================
echo.

echo [1/4] 正在還原 NuGet 套件...
dotnet restore
if %ERRORLEVEL% neq 0 (
    echo 錯誤：套件還原失敗
    pause
    exit /b 1
)

echo.
echo [2/4] 正在建置專案...
dotnet build --configuration Release
if %ERRORLEVEL% neq 0 (
    echo 錯誤：建置失敗
    pause
    exit /b 1
)

echo.
echo [3/4] 正在執行單元測試...
dotnet test
if %ERRORLEVEL% neq 0 (
    echo 警告：測試失敗，但仍會繼續執行應用程式
)

echo.
echo [4/4] 正在啟動應用程式...
echo 提示：應用程式將顯示在桌面右上角
echo 功能：左鍵拖拽移動，右鍵顯示選單
echo.

cd MiniStockView
dotnet run --configuration Release

echo.
echo 應用程式已關閉。
pause
