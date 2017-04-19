using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ListConfigure.converter
{
    [ValueConversion(typeof(string), typeof(string))]
    class StatusToColorConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (((string)value).Contains("Error") || ((string)value).Contains("Fail"))
            {
                return "#cc0000";
            }
            else if ((string)value == "Validated" || (string)value == "Success" || (string)value == "Succeed")
            {
                return "#228b67";
            }
            else return "Gray";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}
