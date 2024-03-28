namespace lab3;

using System.Threading;

public class Data
{
    public static int N { get; set; } = 2000;
    public static int P { get; set; } = 4;
    public int H = N / P;
    public int[,] MO { get; set; } = new int[N, N];
    public int[,] MR { get; set; } = new int[N, N];
    public int[] Z { get; set; } = new int[N];
    public int[] R { get; set; } = new int[N];
    public int[] B { get; set; } = new int[N];
    public int[] X { get; set; } = new int[N];
    public int d { get; set; }
    public int a { get; set; } // Interlocked.Add(ref a, ai);

    public Barrier B1 = new Barrier(4);
    public EventWaitHandle E1 = new EventWaitHandle(false,  EventResetMode.ManualReset);
    public EventWaitHandle E2 = new EventWaitHandle(false,  EventResetMode.ManualReset);
    public EventWaitHandle E3 = new EventWaitHandle(false,  EventResetMode.ManualReset);
    public EventWaitHandle E4 = new EventWaitHandle(false,  EventResetMode.ManualReset);
    public Mutex M1 = new Mutex();
    public Semaphore S1 = new Semaphore(0, 3);
    public object _lockObject = new object();

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
}