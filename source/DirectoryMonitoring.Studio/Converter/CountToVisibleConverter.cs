using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows;

namespace DirectoryMonitoring.Studio.Converter
{
    internal class CountToVisibleConverter : MarkupExtension, IValueConverter
    {
        #region Const

        private const string LABEL = "Log count:";

        #endregion

        #region Implementation IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is string stringValue || value.Equals("0") == false
                ? $"{LABEL} {value}"
                : string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }

        #endregion

        #region Omplementation MarkupExtension

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        #endregion
    }
}
