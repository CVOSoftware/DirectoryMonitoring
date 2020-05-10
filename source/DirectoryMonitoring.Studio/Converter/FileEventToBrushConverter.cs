using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows;
using System.Windows.Media;

namespace DirectoryMonitoring.Studio.Converter
{
    internal class FileEventToBrushConverter : MarkupExtension, IValueConverter
    {
        #region Implementation IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is string eventType)
            {
                switch(eventType)
                {
                    case "Changed":
                        return new SolidColorBrush(Colors.LightBlue);
                    case "Created":
                        return new SolidColorBrush(Colors.Green);
                    case "Deleted":
                        return new SolidColorBrush(Colors.Orange);
                    case "Error":
                        return new SolidColorBrush(Colors.Red);
                    case "Renamed":
                        return new SolidColorBrush(Colors.Violet);
                }
            }

            return default;
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
