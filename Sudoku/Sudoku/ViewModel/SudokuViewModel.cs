using Sudoku.Model;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Sudoku.Helper;
using Sudoku.View;
using Sudoku.Constant;
using Sudoku.SudokuSamples;

namespace Sudoku.ViewModel
{
  class SudokuViewModel : INotifyPropertyChanged
  {
    const int N = 9;
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
          //SelectedSquareChanged();
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
    private int intialIndex;
    public event PropertyChangedEventHandler PropertyChanged;

    public SudokuViewModel()
    {
      NewGameCommand = new DelegateCommand(NewGameCommandExecute);
      GetSolutionCommand = new DelegateCommand(GetSolutionCommandExecute);
      ImportSudokuCommand = new DelegateCommand(ImportSudokuCommandExecute);
      ClearSolutionCommand = new DelegateCommand(ClearSolutionCommandExecute);
      ImportCommand = new DelegateCommand(ImportCommandExecute);
      AutoCheckCommand = new DelegateCommand(AutoCheckCommandExecute);
      InitializeBoard(GetSudokuStringConfiguration());

    }

    private List<List<int>> GetSudokuStringConfiguration(string configuration = null)
    {
      if (configuration == null)
      {
        Random rand = new Random();
        var sudokuSamples = SudokuConfigurations.GetConfigurations;
        var index = rand.Next(sudokuSamples.Count());
        return ParseFromStringToMatrix(sudokuSamples[index]);
      }
      else
      {
        return ParseFromStringToMatrix(configuration);
      }
    }

    private List<List<int>> ParseFromStringToMatrix(string configuration)
    {
      configuration = configuration.Replace('\n', ' ');
      configuration = configuration.Replace(" ", "");
      var matrix = new List<List<int>>();

      int index = 0;
      for (int i = 0; i < N; i++)
      {
        List<int> list = new List<int>();
        for (int j = 0; j < N; j++)
        {
          var number = 0;
          if (configuration[index] > '0' && configuration[index] <= '9')
          {
            number = int.Parse(configuration[index].ToString());
          }
          list.Add(number);
          index++;
        }
        matrix.Add(list);
      }
      return matrix;
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
      for (int i = 0; i < N; i++)
      {
        for (int j = 0; j < N; j++)
        {
          if (Board[i][j].IsReadOnly || Board[i][j].Number == 0)
          {
            continue;
          }

          if (Board[i][j].Number != solutionMatrix[i][j])
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
      for (int i = 0; i < N; i++)
      {
        for (int j = 0; j < N; j++)
        {
          if (Board[i][j].IsReadOnly)
          {
            continue;
          }
          Board[i][j].Foreground = LayoutConstants.DefaultForeground;

        }
      }
      RefreshBoard();
    }

    private void ImportCommandExecute()
    {
      //InitializeBoard(File.ReadAllText(textPath));
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
      InitializeBoard(ParseFromStringToMatrix(e));
    }

    private void ClearSolutionCommandExecute()
    {
      for (int i = 0; i < N; i++)
      {
        for (int j = 0; j < N; j++)
        {
          if (!Board[i][j].IsReadOnly)
            Board[i][j].Number = 0;
        }
      }
    }

    private void GetSolutionCommandExecute()
    {
      for (int i = 0; i < N; i++)
      {
        for (int j = 0; j < N; j++)
        {
          if (!Board[i][j].IsReadOnly)
          {
            Board[i][j].Number = solutionMatrix[i][j]/*.ToString()*/;
            Board[i][j].Foreground = LayoutConstants.DefaultForeground;
          }
        }
      }
      RefreshBoard();
    }

    private void ReadFile(List<List<int>> initialSudoku)
    {
      Board = new ObservableCollection<ObservableCollection<Square>>();
      intMatrix = MatrixService.GetCopyOfMatrix(initialSudoku);

      for (int i = 0; i < N; i++)
      {
        ObservableCollection<Square> line = new ObservableCollection<Square>();
        for (int j = 0; j < N; j++)
        {
          var square = new Square
          {
            Number = intMatrix[i][j],
            Line = i,
            Column = j,
            Tag = i * 10 + j,
            Foreground = intMatrix[i][j] != 0 ? Brushes.Black : LayoutConstants.DefaultForeground,
            IsReadOnly = intMatrix[i][j] != 0,
            BorderThickness = LayoutConstants.GetBorderThickness(i, j),
            Background = LayoutConstants.GetDefaultBackground(i, j)
          };
          //square.NumberChanged += SelectedSquareChanged;
          line.Add(square);
        }
        Board.Add(line);
      }
    }

    private void NumberChangedEvent(object sender, int e)
    {
      var square = sender as Square;
      if (AutoCheckButtonContent == AutoCheckMessage.On)
        SetStyleForAutoCheckerOn();

    }

    public void SelectedSquareChanged(int line, int column, int value)
    {
      Board[line][column].Number = value;
      for (int i = 0; i < N; i++)
      {
        for (int j = 0; j < N; j++)
        {
          if (i == line || j == column)
          {
            Board[i][j].Background = Brushes.LightGray;
          }
          else
          {
            Board[i][j].Background = LayoutConstants.GetDefaultBackground(i, j);
          }

        }
      }
      //int x = SelectedSquare.Number;
      //SelectedSquare.Number = -1;
      //SelectedSquare.Number = x;
      RefreshBoard();
    }

    private void NewGameCommandExecute()
    {
      InitializeBoard(GetSudokuStringConfiguration());
    }

    public void InitializeBoard(List<List<int>> input)
    {
      ReadFile(input);
      SudokuSolver sudokuSolver = new SudokuSolver();
      solutionMatrix = sudokuSolver.GetSolution(intMatrix);
    }

  }
}