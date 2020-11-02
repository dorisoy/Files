using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Files.Converters
{
    class LayoutModeToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value != null)
            {
                int layoutNum = (int)value;
                string iconCode = null;
                switch(layoutNum)
                {
                    case 0:
                        iconCode = "\uF101";
                        break;
                    case 1:
                        iconCode = "\uF100";
                        break;
                    case 2:
                        iconCode = "\uEA72";
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                return iconCode;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
