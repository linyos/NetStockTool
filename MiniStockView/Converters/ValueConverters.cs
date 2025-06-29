using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace MiniStockView.Converters
{
    /// <summary>
    /// 價格變化顏色轉換器
    /// </summary>
    public class PriceChangeColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is decimal change)
            {
                if (change > 0)
                    return new SolidColorBrush(Color.FromRgb(244, 67, 54)); // 紅色 - 上漲
                else if (change < 0)
                    return new SolidColorBrush(Color.FromRgb(76, 175, 80));  // 綠色 - 下跌
                else
                    return new SolidColorBrush(Color.FromRgb(158, 158, 158)); // 灰色 - 平盤
            }
            return new SolidColorBrush(Colors.Gray);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 布爾值轉可見性轉換器
    /// </summary>
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return boolValue ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            }
            return System.Windows.Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 數值格式化轉換器
    /// </summary>
    public class CurrencyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is decimal decimalValue)
            {
                // 根據數值大小決定顯示格式
                if (decimalValue >= 1000)
                    return $"${decimalValue:N0}"; // 大於 1000 顯示千分位，不顯示小數
                else if (decimalValue >= 100)
                    return $"${decimalValue:F1}"; // 100-1000 顯示一位小數
                else
                    return $"${decimalValue:F2}"; // 小於 100 顯示兩位小數
            }
            else if (value is double doubleValue)
            {
                if (doubleValue >= 1000)
                    return $"${doubleValue:N0}";
                else if (doubleValue >= 100)
                    return $"${doubleValue:F1}";
                else
                    return $"${doubleValue:F2}";
            }
            return value?.ToString() ?? string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 百分比格式化轉換器
    /// </summary>
    public class PercentageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is decimal decimalValue)
            {
                var sign = decimalValue >= 0 ? "+" : "";
                return $"{sign}{decimalValue:F2}%";
            }
            else if (value is double doubleValue)
            {
                var sign = doubleValue >= 0 ? "+" : "";
                return $"{sign}{doubleValue:F2}%";
            }
            return value?.ToString() ?? string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
