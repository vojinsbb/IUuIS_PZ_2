using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace NetworkService
{
    public class HeightToTopConverter : IValueConverter
    {
        public double CanvasHeight { get; set; } = 500;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double h)
            {
                return CanvasHeight - h; // stub ide od dna ka gore
            }
            return CanvasHeight; // fallback
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
