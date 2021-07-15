using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Sudoku.Helper
{
  public class Cell
  {
    public int line, column;
  }
  public class SudokuSolver
  {
    public List<Cell> unknownCells;
    public List<List<int>> sudoku;

    public bool IsValidNumber(Cell nr, List<List<int>> sudoku)
    {

      int i, j;
      for (i = 0; i < 9; i++)
      {
        // check if the number already exists in the line
        if (sudoku[nr.line][i] == sudoku[nr.line][nr.column] && i != nr.column)
        {
          return false;
        }
        // check if the number already exists in the column
        if (sudoku[i][nr.column] == sudoku[nr.line][nr.column] && i != nr.line)
        {
          return false;
        }
      }

      int line = (nr.line) / 3 * 3; // first line of the 3x3 square
      int column = (nr.column) / 3 * 3; // first column of the 3x3 square 

      // check if the number already exists in the 3x3 square
      for (i = line; i < line + 3; i++)
      {
        for (j = column; j < column + 3; j++)
        {
          if (i != nr.line && j != nr.column)
          {
            if (sudoku[i][j] == sudoku[nr.line][nr.column])
            {
              return false;
            }
          }
        }
      }

      return true;
    }
    public List<List<int>> GetSolution(List<List<int>> sudokuParameter)
    {
      sudoku = sudokuParameter;
      int index = 0;


      unknownCells = GetUnknownCells(sudoku);
      sudoku[unknownCells[index].line][unknownCells[index].column] = 0;
      while (index > -1)
      {
        if (sudoku[unknownCells[index].line][unknownCells[index].column] < 9)
        {
          sudoku[unknownCells[index].line][unknownCells[index].column]++;
          if (IsValidNumber(unknownCells[index], sudokuParameter))
          {
            if (index == unknownCells.Count() - 1)
            {
              return sudoku;
            }
            else
            {
              index++;
              sudoku[unknownCells[index].line][unknownCells[index].column] = 0;
            }
          }
        }
        else
        {
          sudoku[unknownCells[index].line][unknownCells[index].column] = 0;
          index--;
        }
      }
      return null;
    }

    private List<Cell> GetUnknownCells(List<List<int>> sudoku)
    {
      unknownCells = new List<Cell>();
      for (int i = 0; i < 9; i++)
      {
        for (int j = 0; j < 9; j++)
        {
          if (sudoku[i][j] == 0)
          {
            unknownCells.Add(new Cell { line = i, column = j });
          }
        }
      }
      return unknownCells;
    }
  }
}

