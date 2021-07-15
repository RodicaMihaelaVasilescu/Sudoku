using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Sudoku.Constant
{
  public static class LayoutConstants
  {
    static public Brush ReadOnlyForeground = Brushes.Black;
    static public Brush DefaultForeground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#5c78b8"));

    static public Brush GetDefaultBackground(int i, int j)
    {
      return (i < 3 && j >= 3 && j < 6 || i >= 3 && i < 6 && j < 3 || i >= 3 && i < 6 && j >= 6 || i >= 6 && j >= 3 && j < 6) ?
            Brushes.AliceBlue :
            Brushes.LightSteelBlue;
    }

    static public Thickness GetBorderThickness(int i, int j, int N = 9)
    {
      Thickness borderThickness = new Thickness(1, 1, 1, 1);
      var thickness = 3;
      if (i % 3 == 0)
      {
        borderThickness.Top = thickness;
      }

      if (i == N - 1)
      {
        borderThickness.Bottom = thickness;
      }

      if (j % 3 == 0)
      {
        borderThickness.Left = thickness;
      }

      if (j == N - 1)
      {
        borderThickness.Right = thickness;
      }

      return borderThickness;
    }
  }
}
