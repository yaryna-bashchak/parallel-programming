namespace lab3;

using System.Threading;

public class Data
{
    public static int N { get; set; } = 2000;
    public static int P { get; set; } = 4;
    public static int H = N / P;
    public int[,] MO { get; set; } = new int[N, N];
    public int[,] MR { get; set; } = new int[N, N];
    public int[] Z { get; set; } = new int[N];
    public int[] R { get; set; } = new int[N];
    public int[] B { get; set; } = new int[N];
    public int[] X { get; set; } = new int[N];
    public int d { get; set; }
    public int a = 0; // Interlocked.Add(ref a, ai);

    public Barrier B1 = new Barrier(4);
    public EventWaitHandle E1 = new EventWaitHandle(false, EventResetMode.ManualReset);
    public EventWaitHandle E2 = new EventWaitHandle(false, EventResetMode.ManualReset);
    public EventWaitHandle E3 = new EventWaitHandle(false, EventResetMode.ManualReset);
    public EventWaitHandle E4 = new EventWaitHandle(false, EventResetMode.ManualReset);
    public Mutex M1 = new Mutex();
    public Semaphore S1 = new Semaphore(0, 3);
    public object _lockObject = new object();

    public void FillDataT1()
    {
        for (int i = 0; i < N; i++)
        {
            for (int j = 0; j < N; j++)
            {
                MO[i, j] = 1;
            }
        }
    }

    public void FillDataT2()
    {
        for (int i = 0; i < N; i++)
        {
            Z[i] = 1;
            R[i] = 1;
        }
    }

    public void FillDataT3()
    {
        for (int i = 0; i < N; i++)
        {
            B[i] = 1;
            for (int j = 0; j < N; j++)
            {
                MR[i, j] = 1;
            }
        }
    }

    public void FillDataT4()
    {
        d = 1;
    }

    public static int[,] MultiplyMatrices(int[,] matrix1, int[,] matrix2)
    {
        int m = matrix1.GetLength(0);
        int n = matrix2.GetLength(0);
        int p = matrix2.GetLength(1);

        int[,] result = new int[m, p];

        for (int i = 0; i < m; i++)
        {
            for (int j = 0; j < p; j++)
            {
                result[i, j] = 0;
                for (int k = 0; k < n; k++)
                {
                    result[i, j] += matrix1[i, k] * matrix2[k, j];
                }
            }
        }
        return result;
    }

    public static int MultiplyVectors(int[] vector1, int[] vector2)
    {
        int result = 0;
        for (int i = 0; i < vector1.Length; i++)
        {
            result += vector1[i] * vector2[i];
        }
        return result;
    }

    public static int[] MultiplyVectorByMatrix(int[] vector, int[,] matrix)
    {
        int matrixRows = matrix.GetLength(0);
        int matrixColumns = matrix.GetLength(1);

        int[] result = new int[matrixColumns];

        for (int j = 0; j < matrixColumns; j++)
        {
            result[j] = 0;
            for (int i = 0; i < matrixRows; i++)
            {
                result[j] += vector[i] * matrix[i, j];
            }
        }

        return result;
    }

    public static int[] MultiplyVectorByScalar(int[] vector, int scalar)
    {
        int[] result = new int[vector.Length];

        for (int i = 0; i < vector.Length; i++)
        {
            result[i] = vector[i] * scalar;
        }

        return result;
    }

    public static int[] AddVectors(int[] vector1, int[] vector2)
    {
        int[] result = new int[vector1.Length];

        for (int i = 0; i < vector1.Length; i++)
        {
            result[i] = vector1[i] + vector2[i];
        }

        return result;
    }

    public static int[,] GetSubmatrixFromColumns(int[,] matrix, int threadId)
    {
        int start = Start(threadId);
        int end = End(threadId);

        int rows = matrix.GetLength(0);
        int columns = end - start;
        int[,] submatrix = new int[rows, columns];

        for (int i = 0; i < rows; i++)
        {
            for (int j = start; j < end; j++)
            {
                submatrix[i, j - start] = matrix[i, j];
            }
        }

        return submatrix;
    }

    public static int[] GetSubvector(int[] vector, int threadId)
    {
        int start = Start(threadId);
        int end = End(threadId);

        return vector.Skip(start).Take(end - start).ToArray();
    }

    public static void InsertSubvectorIntoVector(int[] targetVector, int[] subvector, int threadId)
    {
        int start = Start(threadId);
        int size = subvector.GetLength(0);

        for (int i = 0; i < size; i++)
        {
            targetVector[start + i] = subvector[i];
        }
    }

    public static void PrintVector(int[] vector)
    {
        for (int i = 0; i < vector.Length; i++)
        {
            Console.Write(vector[i] + " ");
        }
        Console.WriteLine();
    }

    private static int Start(int threadId) => (threadId - 1) * H;
    private static int End(int threadId) => threadId * H;

    // public static void InsertSubmatrixIntoMatrix(int[,] targetMatrix, int[,] submatrix, int threadId)
    // {
    //     int start = Start(threadId);

    //     int rows = targetMatrix.GetLength(0);
    //     int submatrixColumns = submatrix.GetLength(1);

    //     for (int i = 0; i < rows; i++)
    //     {
    //         for (int j = 0; j < submatrixColumns; j++)
    //         {
    //             targetMatrix[i, start + j] = submatrix[i, j];
    //         }
    //     }
    // }
}