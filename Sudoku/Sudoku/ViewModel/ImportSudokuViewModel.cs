using Prism.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Sudoku.ViewModel
{
  class ImportSudokuViewModel : INotifyPropertyChanged
  {
    private string _sudokuInput= "0 6 0 1 0 4 0 5 0\n0 0 8 3 0 5 6 0 0\n2 0 0 0 0 0 0 0 1\n8 0 0 4 0 7 0 0 6\n0 0 6 0 0 0 3 0 0\n7 0 0 9 0 1 0 0 4\n5 0 0 0 0 0 0 0 2\n0 0 7 2 0 6 9 0 0\n0 4 0 5 0 8 0 7 0";

    public Action CloseAction { get; set; }
    public ICommand ImportCommand { get; set; }
    public ICommand CancelCommand { get; set; }
    public string SudokuInput
    {
      get { return _sudokuInput; }

      set
      {
        if (_sudokuInput == value) return;
        _sudokuInput = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SudokuInput"));
      }

    }
    public EventHandler<string> ImportEvent { get; set; }
    public ImportSudokuViewModel()
    {
      ImportCommand = new DelegateCommand(ImportCommandExecute);
      CancelCommand = new DelegateCommand(CancelCommandExecute);
    }

    public event PropertyChangedEventHandler PropertyChanged;

    private void CancelCommandExecute()
    {
      CloseAction?.Invoke();
    }

    private void ImportCommandExecute()
    {
      ImportEvent?.Invoke(this, SudokuInput);
      CloseAction?.Invoke();
    }
  }
}
