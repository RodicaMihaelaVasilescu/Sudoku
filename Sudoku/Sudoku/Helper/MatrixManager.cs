using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku.Helper
{
  public static class MatrixService
  {
    private static int N = 9;
    public static List<List<int>> GetCopyOfMatrix(List<List<int>> matrix)
    {
      var newMatrix = new List<List<int>>();
      for (int i = 0; i < N; i++)
      {
        var line = new List<int>();
        for (int j = 0; j < N; j++)
        {
          line.Add(matrix[i][j]);
        }
        newMatrix.Add(line);
      }
      return newMatrix;
    }
  }
}
