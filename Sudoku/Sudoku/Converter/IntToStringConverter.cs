using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Sudoku.Converter
{
  public class IntToStringConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if ((int)value == 0)
        value = " ";
      return value.ToString();
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      int ret = 0;
      int.TryParse((string)value, out ret);
      ret %= 10;
      return ret;
    }
  }

}
