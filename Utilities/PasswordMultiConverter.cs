using System;
using System.Globalization;
using System.Windows.Data;

namespace QuanLyNhaHang.Utilities
{
    public class PasswordMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            // Trả về mảng các PasswordBox để ViewModel xử lý
            return values.Clone();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}