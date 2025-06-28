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
                    return new SolidColorBrush(Color.FromRgb(76, 175, 80)); // 綠色
                else if (change < 0)
                    return new SolidColorBrush(Color.FromRgb(244, 67, 54));  // 紅色
                else
                    return new SolidColorBrush(Color.FromRgb(158, 158, 158)); // 灰色
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
                return decimalValue.ToString("C2", culture);
            }
            else if (value is double doubleValue)
            {
                return doubleValue.ToString("C2", culture);
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
