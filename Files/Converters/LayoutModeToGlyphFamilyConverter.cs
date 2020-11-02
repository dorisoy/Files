using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace Files.Converters
{
    class LayoutModeToGlyphFamilyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value != null)
            {
                int layoutNum = (int)value;
                FontFamily fontFamilyResource = null;
                fontFamilyResource = layoutNum switch
                {
                    0 => App.Current.Resources["CustomGlyph"] as FontFamily,
                    1 => App.Current.Resources["CustomGlyph"] as FontFamily,
                    2 => App.Current.Resources["FluentUIGlyphs"] as FontFamily,
                    _ => throw new ArgumentOutOfRangeException(),
                };
                return fontFamilyResource;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
