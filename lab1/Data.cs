namespace lab1;

public class Data
{
    public int N { get; private set; }
    public int[] A { get; private set; }
    public int[] B { get; private set; }
    public int[] C { get; private set; }
    public int[,] MA { get; private set; }
    public int[,] ME { get; private set; }
    public int[,] MF { get; private set; }
    public int[,] MG { get; private set; }
    public int[,] ML { get; private set; }
    public int[,] MR { get; private set; }
    public int[,] MT { get; private set; }
    public int[] O { get; private set; }
    public int[] P { get; private set; }
    private readonly Random random = new();

    public Data(int n)
    {
        N = n;
        A = new int[N];
        B = new int[N];
        C = new int[N];
        MA = new int[N, N];
        ME = new int[N, N];
        MF = new int[N, N];
        MG = new int[N, N];
        ML = new int[N, N];
        MR = new int[N, N];
        MT = new int[N, N];
        O = new int[N];
        P = new int[N];
    }

    public void InitializeForFunc1()
    {
        if (N < 100)
        {
            for (int i = 0; i < N; i++)
            {
                A[i] = ReadValue($"A[{i}]", 1);
                B[i] = ReadValue($"B[{i}]", 1);
                C[i] = ReadValue($"C[{i}]", 1);
                for (int j = 0; j < N; j++)
                {
                    MA[i, j] = ReadValue($"MA[{i},{j}]", 1);
                    ME[i, j] = ReadValue($"ME[{i},{j}]", 1);
                }
            }
        }
        else
        {
            var filePath = "func1_data.txt";
            if (File.Exists(filePath))
            {
                LoadFromFile(filePath, new int[][] { A, B, C }, new int[][,] { MA, ME });
            }
            else
            {
                // якщо файлу нема, всі елементи задаються або фіксованим числом, наприклад, 1, або рандомним чином.
                // щоб змінити спосіб задання потрібно розкоментувати і закоментувати виклики відповідних функцій нижче

                var fixedValue = 1;
                InitializeWithFixedValue(new int[][] { A, B, C }, new int[][,] { MA, ME }, fixedValue, filePath);
                // InitializeWithRandomValues(new int[][] { A, B, C }, new int[][,] { MA, ME }, filePath);
            }
        }
    }

    public void InitializeForFunc2()
    {
        if (N < 100)
        {
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    MF[i, j] = ReadValue($"MF[{i},{j}]", 2);
                    MG[i, j] = ReadValue($"MG[{i},{j}]", 2);
                    ML[i, j] = ReadValue($"ML[{i},{j}]", 2);
                }
            }
        }
        else
        {
            var filePath = "func2_data.txt";
            if (File.Exists(filePath))
            {
                LoadFromFile(filePath, Array.Empty<int[]>(), new int[][,] { MF, MG, ML });
            }
            else
            {
                // якщо файлу нема, всі елементи задаються або фіксованим числом, наприклад, 2, або рандомним чином.
                // щоб змінити спосіб задання потрібно розкоментувати і закоментувати виклики відповідних функцій нижче

                // var fixedValue = 2;
                // InitializeWithFixedValue(Array.Empty<int[]>(), new int[][,] { MF, MG, ML }, fixedValue, filePath);
                InitializeWithRandomValues(Array.Empty<int[]>(), new int[][,] { MF, MG, ML }, filePath);
            }
        }
    }

    public void InitializeForFunc3()
    {
        if (N < 100)
        {
            for (int i = 0; i < N; i++)
            {
                O[i] = ReadValue($"O[{i}]", 3);
                P[i] = ReadValue($"P[{i}]", 3);
                for (int j = 0; j < N; j++)
                {
                    MR[i, j] = ReadValue($"MR[{i},{j}]", 3);
                    MT[i, j] = ReadValue($"MT[{i},{j}]", 3);
                }
            }
        }
        else
        {
            var filePath = "func3_data.txt";
            if (File.Exists(filePath))
            {
                LoadFromFile(filePath, new int[][] { O, P }, new int[][,] { MR, MT });
            }
            else
            {
                // якщо файлу нема всі елементи задаються або фіксованим числом, наприклад, 3, або рандомним чином.
                // щоб змінити спосіб задання потрібно розкоментувати і закоментувати виклики відповідних функцій нижче

                var fixedValue = 3;
                InitializeWithFixedValue(new int[][] { O, P }, new int[][,] { MR, MT }, fixedValue, filePath);
                // InitializeWithRandomValues(new int[][] { O, P }, new int[][,] { MR, MT }, filePath);
            }
        }
    }

    private void LoadFromFile(string filePath, int[][] vectors, int[][,] matrices)
    {
        string[] lines = File.ReadAllLines(filePath);

        int lineIndex = 1;
        // читаємо вектори
        foreach (var vector in vectors)
        {
            string[] row = lines[lineIndex].Split(',');
            for (int i = 0; i < N; i++)
            {
                vector[i] = int.Parse(row[i]);
            }
            lineIndex += 2;
        }

        // читаємо матриці
        foreach (var matrix in matrices)
        {
            for (int i = 0; i < N; i++)
            {
                string[] row = lines[lineIndex++].Split(',');
                for (int j = 0; j < N; j++)
                {
                    matrix[i, j] = int.Parse(row[j]);
                }
            }
            lineIndex++;
        }

        Console.WriteLine($"Data from {filePath} was read");
    }

    private void InitializeWithFixedValue(int[][] vectors, int[][,] matrices, int fixedValue, string filePath)
    {
        foreach (var vector in vectors)
        {
            for (int i = 0; i < N; i++)
            {
                vector[i] = fixedValue;
            }
        }

        foreach (var matrix in matrices)
        {
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    matrix[i, j] = fixedValue;
                }
            }
        }

        Console.WriteLine($"File {filePath} not found, so data was set to fixed value {fixedValue}");
    }

    private void InitializeWithRandomValues(int[][] vectors, int[][,] matrices, string filePath)
    {
        foreach (var vector in vectors)
        {
            for (int i = 0; i < N; i++)
            {
                vector[i] = random.Next(1, 4);
            }
        }

        foreach (var matrix in matrices)
        {
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    matrix[i, j] = random.Next(1, 4);
                }
            }
        }

        Console.WriteLine($"File {filePath} not found, so data was randomly provided");
    }

    public void OutputResults(string output, string fileName)
    {
        if (N < 100)
        {
            Console.WriteLine(output);
        }
        else
        {
            File.WriteAllText(fileName, output);
        }
    }

    public int[] F1()
    {
        // E = A + C *(MA*ME) + B
        int[] E = new int[N];
        int[,] MAMEResult = MultiplyMatrices(MA, ME);
        int[] CMAMEResult = MultiplyMatrixVector(MAMEResult, C);

        for (int i = 0; i < N; i++)
        {
            E[i] = A[i] + CMAMEResult[i] + B[i];
        }

        return E;
    }

    public int F2()
    {
        // k = MAX(MF + MG*ML)
        int[,] MGMLResult = MultiplyMatrices(MG, ML);
        int[,] MFPlusMGML = new int[N, N];

        for (int i = 0; i < N; i++)
        {
            for (int j = 0; j < N; j++)
            {
                MFPlusMGML[i, j] = MF[i, j] + MGMLResult[i, j];
            }
        }

        return FindMax(MFPlusMGML);
    }

    public int[] F3()
    {
        // S = (O+P)*TRANS(MR * MT)
        int[,] MRMTResult = MultiplyMatrices(MR, MT);
        int[,] MRMTTransposed = TransposeMatrix(MRMTResult);
        int[] OPResult = new int[N];

        for (int i = 0; i < N; i++)
        {
            OPResult[i] = O[i] + P[i];
        }

        return MultiplyMatrixVector(MRMTTransposed, OPResult);
    }

    private static int[] MultiplyMatrixVector(int[,] matrix, int[] vector)
    {
        int n = matrix.GetLength(0);

        int[] result = new int[n];
        for (int i = 0; i < n; i++)
        {
            result[i] = 0;
            for (int j = 0; j < n; j++)
            {
                result[i] += matrix[i, j] * vector[j];
            }
        }
        return result;
    }

    private static int[,] MultiplyMatrices(int[,] matrix1, int[,] matrix2)
    {
        int n = matrix1.GetLength(0);

        int[,] result = new int[n, n];
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
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

    private static int FindMax(int[,] matrix)
    {
        int n = matrix.GetLength(0);

        int max = matrix[0, 0];
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                if (matrix[i, j] > max)
                {
                    max = matrix[i, j];
                }
            }
        }
        return max;
    }

    private static int[,] TransposeMatrix(int[,] matrix)
    {
        int n = matrix.GetLength(0);

        int[,] transposed = new int[n, n];
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                transposed[j, i] = matrix[i, j];
            }
        }
        return transposed;
    }

    private int ReadValue(string prompt, int defaultValue)
    {
        Console.Write($"{prompt}: ");
        if (int.TryParse(Console.ReadLine(), out int value))
        {
            return value;
        }
        else
        {
            return defaultValue;
        }
    }
}
