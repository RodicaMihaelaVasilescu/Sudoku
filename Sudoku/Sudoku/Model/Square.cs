using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.ComponentModel;
using System.Windows;
using System;

namespace Sudoku.Model
{
  public class Square : INotifyPropertyChanged
  {
    private double _size = 80;
    private int _number;
    private string _selectedSquare;

    public int Line { get; set; }
    public int Column { get; set; }
    public int Tag { get; set; }
    public Brush Background { get; set; } = new SolidColorBrush(Color.FromRgb(0xa4, 0Xde, 0Xda)); // #a4deda
    public Brush Foreground { get; set; } = Brushes.Gray; // #7c7064
    public Thickness BorderThickness { get; set; } = new Thickness(3, 5, 8, 0);
    public EventHandler<int> NumberChanged { get; set; }

    public string SelectedSquare
    {
      get { return _selectedSquare; }

      set
      {
        if (_selectedSquare == value) return;
        _selectedSquare = value;
        if (SelectedSquare != null)
        {
          //SelectedSquareChanged();
        }
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedSquare"));
      }

    }
    public int Number
    {
      get { return _number; }

      set
      {
        if (_number == value || IsReadOnly) return;
        //NumberChanged?.Invoke(this, value);
        _number = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Number"));
      }
    }
    public bool IsReadOnly { get; set; } = false;

    public event PropertyChangedEventHandler PropertyChanged;
  }
}