using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Globalization;

namespace Visual_Matrix.Converters
{
    public class IntToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int val)
            {
                switch (val)
                {
                    case 1: return Brushes.Red;
                    case 2: return Brushes.Green;
                    default: return Brushes.Black;
                }
            }
            else return Brushes.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException(); // Нам обратное преобразование не нужно
        }
    }
}
