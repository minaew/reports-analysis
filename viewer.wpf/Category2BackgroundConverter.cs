using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using PdfExtractor.Models;

namespace Viewer.Wpf
{
    public class Category2BackgroundConverter : IValueConverter
    {
        private readonly Brush _unknownCategoryBrush = new SolidColorBrush(Colors.Salmon);
        private readonly Brush _knownCategoryBrush = new SolidColorBrush(Colors.White);
        
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Operation operation)
            {
                return operation.IsUnknownCategory ? _unknownCategoryBrush : _knownCategoryBrush;
            }

            return _knownCategoryBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}