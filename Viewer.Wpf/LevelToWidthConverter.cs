﻿using System;
using System.Globalization;
using System.Windows.Data;

namespace ReportAnalysis.Viewer.Wpf
{
    [ValueConversion(typeof(int), typeof(double))]
    public class LevelToWidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var level = (int)value;
            return 800 - level * 20;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
