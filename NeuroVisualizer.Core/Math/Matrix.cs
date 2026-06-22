using System.Data;
using System.Text;
public class Matrix {

    public int Rows { get { return Data.GetLength(0); } }
    public int Cols { get { return Data.GetLength(1); } }
    public double[,] Data { get; }

    public Matrix(int rows,int cols)
    {
        Data = new double[rows, cols];
        // Data2 = new double[rows * cols];
    }

    public Matrix (double [,] data)
    {
        Data = data;
    }

    public static Matrix Random(int rows, int cols, Random rng)
    {
        var m = new Matrix(rows, cols);
        for (int r = 0; r < rows; r++) {
            for (int c = 0; c < cols; c++) {
                m.Data[r, c] = rng.NextDouble() * 2 - 1;

            }
        }
        return m;
    }

    public void Print()
    {
        for (int r = 0; r < Rows; r++)
        {
            for (int c = 0; c < Cols; c++)
            {
                Console.Write($"{Data[r, c]:F3}\t");
                // Console.Write($"{Data2[r * Cols + c]:F3}\t");
            }
            Console.WriteLine();
        }
    }


    public static Matrix addMatrix(Matrix A, Matrix B)
    {
        int r = A.Rows;
        int c = A.Cols;
        Matrix resultMatrix = new Matrix(A.Rows, A.Cols);
        for (int i = 0; i < r; i++)
        {
            for (int j = 0; j < c; j++)
            {
                resultMatrix.Data[i,j] = A.Data[i,j] + B.Data[i,j];
            }
        }
        return resultMatrix;
    }


    public static Matrix subMatrix(Matrix A, Matrix B)
    {
        int r = A.Rows;
        int c = A.Cols;

        Matrix resultMatrix = new Matrix(A.Rows, A.Cols);
        for (int i = 0; i < r; i++)
        {
            for (int j = 0; j < c; j++)
            {
                resultMatrix.Data[i, j] = A.Data[i, j] - B.Data[i, j];
                // resultMatrix.Data[resultMatrix.Rows * i + j] = A.Data2[A.Rows * i + j] - B.Data2[B.Rows * i + j];
            }
        }
        return resultMatrix;
    }


    public static Matrix scalarMatrix(Matrix A, double scalar)
    {
        int r = A.Rows;
        int c = A.Cols;

        Matrix resultMatrix = new Matrix(A.Rows, A.Cols);
        for (int i = 0; i < r; i++)
        {
            for (int j = 0; j < c; j++)
            {
                resultMatrix.Data[i, j] = A.Data[i, j] * scalar;
                // resultMatrix.Data2[i * c + j] = A.Data2[i * c + j] * scalar;
            }
        }
        return resultMatrix;
    }


    public static Matrix transposeMatrix(Matrix A)
    {
        int r = A.Rows;
        int c = A.Cols;

        Matrix resultMatrix = new Matrix(A.Cols, A.Rows);
        for (int i = 0; i < c; i++)
        {
            for (int j = 0; j < r; j++)
            {
                resultMatrix.Data[i, j] = A.Data[j, i];
            }
        }
        return resultMatrix;
    }


    public static Matrix mulMatrix(Matrix A, Matrix B)
    {
        if (A.Cols != B.Rows)
        {
            throw new ArgumentException("Invalid dimensions for multipication");
        }
        int rA = A.Rows;
        int cA = A.Cols;
        int cB = B.Cols;

        Matrix resultMatrix = new Matrix(A.Rows, B.Cols);
        for (int i = 0; i < rA; i++)
        {
            for (int j = 0; j < cB; j++)
            {
                for (int k = 0; k < cA; k++)
                {
                    resultMatrix.Data[i, j] += A.Data[i, k] * B.Data[k, j];
                }
            }
        }
        return resultMatrix;
    }




    public static Matrix elementWiseMulMatrix(Matrix A, Matrix B)
    {
        int rA = A.Rows;
        int cA = A.Cols;
        int rB = B.Rows;
        int cB = B.Cols;
        if (rA != rB || cA != cB )
        {
            throw new ArgumentException("Matrixes should have the same dimensions for element wise multiplication");
        }

        Matrix resultMatrix = new Matrix(A.Rows, A.Cols);
        for (int i = 0; i < rA; i++)
        {
            for (int j = 0; j < cA; j++)
            {
                resultMatrix.Data[i, j] = A.Data[i, j] * B.Data[i, j];
            }
        }
        return resultMatrix;
    }


    public static Matrix mapMatrix(Matrix A, Func<double, double> f)
    {
        int r = A.Rows;
        int c = A.Cols;

        Matrix resultMatrix = new Matrix(A.Rows, A.Cols);

        for (int i = 0; i < r; i++)
        {
            for (int j = 0; j < c; j++)
            {
                resultMatrix.Data[i, j] = f(A.Data[i, j]);
            }
        }
        return resultMatrix;
    }


    // helper method to help with saving weights and biases to file
    public static String matrixToString(Matrix m)
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < m.Rows; i++)
        {
            for (int j = 0; j < m.Cols; j++)
            {
                sb.Append(m.Data[i, j].ToString("R"));
                if (j < m.Cols - 1)
                {
                    sb.Append(",");
                }
            }
            sb.AppendLine();
        }
        return sb.ToString();
    }

    // helper method to help with loading weights and biases from file text -> matrix
    public static Matrix stringToMatrix(String text)
    {
        StringBuilder sb = new StringBuilder();
        string[] lines = text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
        int rows = lines.Length;
        int cols = lines[0].Split(',').Length;
        Matrix m = new Matrix(rows, cols);
        for (int i = 0; i < rows; i++)
        {
            string[] values = lines[i].Split(',');
            for (int j = 0; j < cols; j++)
            {
                m.Data[i, j] = double.Parse(values[j]);
            }
        }
        return m;
    }
}
