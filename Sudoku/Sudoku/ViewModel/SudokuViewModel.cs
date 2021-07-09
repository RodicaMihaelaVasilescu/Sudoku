using Sudoku.Model;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows;
using System.IO;
using Sudoku.Helper;
using Sudoku.View;
using Sudoku.Constant;

namespace Sudoku.ViewModel
{
  class SudokuViewModel : INotifyPropertyChanged
  {
    int n = 9;
    private readonly string textPath = @"..\..\SudokuSamples\sudoku.txt";
    public ObservableCollection<ObservableCollection<Square>> Board
    {
      get { return _board; }

      set
      {
        if (_board == value) return;
        _board = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Board"));
      }

    }

    public Square SelectedSquare
    {
      get { return _selectedSquare; }

      set
      {
        if (_selectedSquare == value) return;
        _selectedSquare = value;
        if (SelectedSquare != null)
        {
          SelectedSquareChanged();
        }
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedSquare"));
      }

    }

    public string AutoCheckButtonContent
    {
      get { return _autoCheckButtonContent; }

      set
      {
        if (_autoCheckButtonContent == value) return;
        _autoCheckButtonContent = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("AutoCheckButtonContent"));
      }

    }


    public ICommand NewGameCommand { get; set; }
    public ICommand GetSolutionCommand { get; set; }
    public ICommand ClearSolutionCommand { get; set; }
    public ICommand ImportSudokuCommand { get; set; }
    public ICommand ImportCommand { get; set; }
    public ICommand AutoCheckCommand { get; set; }

    private ObservableCollection<ObservableCollection<Square>> _board;

    private List<List<int>> intMatrix;
    private List<List<int>> solutionMatrix;
    private List<List<int>> initialSudoku;
    private string _autoCheckButtonContent = AutoCheckMessage.On;
    private Square _selectedSquare;

    public event PropertyChangedEventHandler PropertyChanged;

    public SudokuViewModel()
    {
      NewGameCommand = new DelegateCommand(NewGameCommandExecute);
      GetSolutionCommand = new DelegateCommand(GetSolutionCommandExecute);
      ImportSudokuCommand = new DelegateCommand(ImportSudokuCommandExecute);
      ClearSolutionCommand = new DelegateCommand(ClearSolutionCommandExecute);
      ImportCommand = new DelegateCommand(ImportCommandExecute);
      AutoCheckCommand = new DelegateCommand(AutoCheckCommandExecute);
      InitializeBoard(File.ReadAllText(textPath));

    }

    private void AutoCheckCommandExecute()
    {
      //if (AutoCheckButtonContent == AutoCheckMessage.On)
      //{
      //  AutoCheckButtonContent = AutoCheckMessage.Off;
      //  SetStyleForAutoCheckerOff();

      //}
      //else
      //{
      //AutoCheckButtonContent = AutoCheckMessage.On;
      SetStyleForAutoCheckerOn();
      //}
    }

    private void SetStyleForAutoCheckerOn()
    {
      for (int i = 0; i < n; i++)
      {
        for (int j = 0; j < n; j++)
        {
          if (Board[i][j].IsReadOnly)
          {
            continue;
          }
          if (Board[i][j].Number != solutionMatrix[i][j] && Board[i][j].Number != 0)
          {
            Board[i][j].Foreground = Brushes.Red;
          }
          else
          {
            Board[i][j].Foreground = Brushes.Green;
          }
        }
      }
      RefreshBoard();
    }

    void RefreshBoard()
    {
      CollectionViewSource.GetDefaultView(Board).Refresh();
    }

    private void SetStyleForAutoCheckerOff()
    {
      for (int i = 0; i < n; i++)
      {
        for (int j = 0; j < n; j++)
        {
          if (Board[i][j].IsReadOnly)
          {
            continue;
          }
          Board[i][j].Foreground = Brushes.DarkGray;

        }
      }
      RefreshBoard();
    }

    private void ImportCommandExecute()
    {
      ReadFile(File.ReadAllText(textPath));
      InitializeBoard(File.ReadAllText(textPath));
    }

    private void ImportSudokuCommandExecute()
    {
      var window = new ImportSudokuView();
      var viewModel = new ImportSudokuViewModel();
      window.DataContext = viewModel;
      if (viewModel.CloseAction == null)
      {
        viewModel.CloseAction = () => window.Close();
      }
      viewModel.ImportEvent += ImportSudokuInput;
      window.Show();
    }

    private void ImportSudokuInput(object sender, string e)
    {
      InitializeBoard(e);
    }

    private void ClearSolutionCommandExecute()
    {
      for (int i = 0; i < n; i++)
      {
        for (int j = 0; j < n; j++)
        {
          if (!Board[i][j].IsReadOnly)
            Board[i][j].Number = 0;
        }
      }
    }

    private List<List<int>> GetCopyOfMatrix(List<List<int>> matrix)
    {
      var newMatrix = new List<List<int>>();
      for (int i = 0; i < n; i++)
      {
        var line = new List<int>();
        for (int j = 0; j < n; j++)
        {
          line.Add(matrix[i][j]);
        }
        newMatrix.Add(line);
      }
      return newMatrix;
    }

    private void GetSolutionCommandExecute()
    {
      for (int i = 0; i < n; i++)
      {
        for (int j = 0; j < n; j++)
        {
          if (!Board[i][j].IsReadOnly)
          {
            Board[i][j].Number = solutionMatrix[i][j]/*.ToString()*/;
            Board[i][j].Foreground = Brushes.DarkGray;
          }
        }
      }
      RefreshBoard();
    }

    private void ReadFile(string input)
    {
      if (input == null)
      {
        return;
      }

      Board = new ObservableCollection<ObservableCollection<Square>>();
      intMatrix = new List<List<int>>();
      for (int k = 0; k < n; k++)
      {
        intMatrix.Add(new List<int> { 0, 0, 0, 0, 0, 0, 0, 0, 0 });
      }

      int i = 0;
      foreach (var row in input.Split('\n'))
      {
        int j = 0;
        ObservableCollection<Square> line = new ObservableCollection<Square>();
        foreach (var col in row.Trim().Split(' '))
        {
          int number = int.Parse(col.Trim());
          intMatrix[i][j] = number;
          var square = new Square
          {
            Number = number,
            Line = i,
            Column = j,
            Foreground = number == 0 ? Brushes.DarkGray : Brushes.Black,
            IsReadOnly = number != 0,
            BorderThickness = GetBorderThickness(i, j),
            Background = GetBackground(i, j)
          };

          //square.NumberChanged += NumberChangedEvent;
          line.Add(square);
          j++;
        }
        Board.Add(line);
        i++;
      }
      initialSudoku = GetCopyOfMatrix(intMatrix);

    }

    private void NumberChangedEvent(object sender, int e)
    {
      var square = sender as Square;
      if (AutoCheckButtonContent == AutoCheckMessage.On)
        SetStyleForAutoCheckerOn();

    }

    private void SelectedSquareChanged()
    {
      if (SelectedSquare.Number == 0)
        return;
      for (int i = 0; i < n; i++)
      {
        for (int j = 0; j < n; j++)
        {
          if (i == SelectedSquare.Line || j == SelectedSquare.Column)
          {
            Board[i][j].Background = Brushes.LightGray;
          }
          else
          {
            Board[i][j].Background = GetBackground(i, j);
          }

        }
      }
      RefreshBoard();
    }

    private void NewGameCommandExecute()
    {
      InitializeBoard(File.ReadAllText(textPath));
    }

    public void InitializeBoard(string input)
    {
      ReadFile(input);
      SudokuSolver sudokuSolver = new SudokuSolver();
      solutionMatrix = sudokuSolver.GetSolution(intMatrix);
    }


    private bool IsEnabled(int i, int j)
    {
      return i + j % 2 == 0;
    }

    private Brush GetBackground(int i, int j)
    {
      return (i < 3 && j >= 3 && j < 6 || i >= 3 && i < 6 && j < 3 || i >= 3 && i < 6 && j >= 6 || i >= 6 && j >= 3 && j < 6) ?
            Brushes.AliceBlue :
            Brushes.LightBlue;
    }

    private Thickness GetBorderThickness(int i, int j)
    {
      Thickness borderThickness = new Thickness(1, 1, 1, 1);
      var thickness = 3;
      if (i % 3 == 0)
      {
        borderThickness.Top = thickness;
      }

      if (i == n - 1)
      {
        borderThickness.Bottom = thickness;
      }

      if (j % 3 == 0)
      {
        borderThickness.Left = thickness;
      }

      if (j == n - 1)
      {
        borderThickness.Right = thickness;
      }

      return borderThickness;
    }
  }
}