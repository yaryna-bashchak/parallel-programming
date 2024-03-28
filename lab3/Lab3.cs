// Програмне забезпечення високопродуктивних комп'ютерних систем
// Лабораторна робота №3: програмування для КС із СП. Семафори, мютекси, події, критичні секції, атомік-змінні, бар’єри
// варіант 20
// X = (B*Z)*(d*Z + R*(MO*MR))
// Бащак Ярина Володимирівна
// група ІМ-11
// 28.03.2024

using System.Diagnostics;
using lab3;
class Lab3
{
    private static readonly Data data = new Data();
    private Stopwatch stopwatch = new Stopwatch();

    static void Main(string[] args)
    {
        // для виконання тільки на одному ядрі
        // Process process = Process.GetCurrentProcess();
        // process.ProcessorAffinity = (IntPtr)0x0001;

        Lab3 lab3Instance = new Lab3();
        lab3Instance.stopwatch.Start();

        var t1 = new Thread(new ParameterizedThreadStart(lab3Instance.T1));
        var t2 = new Thread(new ParameterizedThreadStart(lab3Instance.T2));
        var t3 = new Thread(new ParameterizedThreadStart(lab3Instance.T3));
        var t4 = new Thread(new ParameterizedThreadStart(lab3Instance.T4));

        t1.Start(1);
        t2.Start(2);
        t3.Start(3);
        t4.Start(4);

        t1.Join();
        t2.Join();
        t3.Join();
        t4.Join();
    }

    void T1(object param)
    {
        int threadId = (int)param;
        int a1, d1;
        Console.WriteLine($"T{threadId} start");

        // введення МО
        data.FillDataT1();
        // синхронізація введення за допомогою бар'єру B1
        data.B1.SignalAndWait();

        // обчислення 1: a1 = Bн*Zн
        a1 = Data.MultiplyVectors(Data.GetSubvector(data.B, threadId), Data.GetSubvector(data.Z, threadId));
        // обчислення 2: а = а + а1 - КД1 - атомік-змінна
        Interlocked.Add(ref data.a, a1);

        // сигнал про завершення обчислення 2 - подія Е1
        data.E1.Set();
        // чекати, щоб всі потоки завершили обчислення 2 - події Е2, Е3, Е4
        data.E2.WaitOne();
        data.E3.WaitOne();
        data.E4.WaitOne();

        // захист скаляру d за допомогою мютексу М1 - КД2
        data.M1.WaitOne();
        d1 = data.d;
        data.M1.ReleaseMutex();

        // захист скаляру а за допомогою критичної секції - КД3
        lock (data._lockObject)
        {
            a1 = data.a;
        }

        // обчислення 3: Xн = a1*(d1*Zн + R*(MO*MRн))
        data.CalculateStep3(a1, d1, threadId);

        // сигнал задачі Т4 про завершення обчислення 3 - семафор S1
        data.S1.Release();
        Console.WriteLine($"T{threadId} finish");
    }

    void T2(object param)
    {
        int threadId = (int)param;
        int a2, d2;
        Console.WriteLine($"T{threadId} start");

        // введення Z, R
        data.FillDataT2();
        // синхронізація введення за допомогою бар'єру B1
        data.B1.SignalAndWait();

        // обчислення 1: a2 = Bн*Zн
        a2 = Data.MultiplyVectors(Data.GetSubvector(data.B, threadId), Data.GetSubvector(data.Z, threadId));
        // обчислення 2: а = а + а2 - КД1 - атомік-змінна
        Interlocked.Add(ref data.a, a2);

        // сигнал про завершення обчислення 2 - подія Е2
        data.E2.Set();
        // чекати, щоб всі потоки завершили обчислення 2 - події Е1, Е3, Е4
        data.E1.WaitOne();
        data.E3.WaitOne();
        data.E4.WaitOne();

        // захист скаляру d за допомогою мютексу М1 - КД2
        data.M1.WaitOne();
        d2 = data.d;
        data.M1.ReleaseMutex();

        // захист скаляру а за допомогою критичної секції - КД3
        lock (data._lockObject)
        {
            a2 = data.a;
        }

        // обчислення 3: Xн = a2*(d2*Zн + R*(MO*MRн))
        data.CalculateStep3(a2, d2, threadId);

        // сигнал задачі Т4 про завершення обчислення 3 - семафор S1
        data.S1.Release();
        Console.WriteLine($"T{threadId} finish");
    }

    void T3(object param)
    {
        int threadId = (int)param;
        int a3, d3;
        Console.WriteLine($"T{threadId} start");

        // введення B, MR
        data.FillDataT3();
        // синхронізація введення за допомогою бар'єру B1
        data.B1.SignalAndWait();

        // обчислення 1: a3 = Bн*Zн
        a3 = Data.MultiplyVectors(Data.GetSubvector(data.B, threadId), Data.GetSubvector(data.Z, threadId));
        // обчислення 2: а = а + а3 - КД1 - атомік-змінна
        Interlocked.Add(ref data.a, a3);

        // сигнал про завершення обчислення 2 - подія Е3
        data.E3.Set();
        // чекати, щоб всі потоки завершили обчислення 2 - події Е1, Е2, Е4
        data.E1.WaitOne();
        data.E2.WaitOne();
        data.E4.WaitOne();

        // захист скаляру d за допомогою мютексу М1 - КД2
        data.M1.WaitOne();
        d3 = data.d;
        data.M1.ReleaseMutex();

        // захист скаляру а за допомогою критичної секції - КД3
        lock (data._lockObject)
        {
            a3 = data.a;
        }

        // обчислення 3: Xн = a3*(d3*Zн + R*(MO*MRн))
        data.CalculateStep3(a3, d3, threadId);

        // сигнал задачі Т4 про завершення обчислення 3 - семафор S1
        data.S1.Release();
        Console.WriteLine($"T{threadId} finish");
    }

    void T4(object param)
    {
        int threadId = (int)param;
        int a4, d4;
        Console.WriteLine($"T{threadId} start");

        // введення d
        data.FillDataT4();
        // синхронізація введення за допомогою бар'єру B1
        data.B1.SignalAndWait();

        // обчислення 1: a4 = Bн*Zн
        a4 = Data.MultiplyVectors(Data.GetSubvector(data.B, threadId), Data.GetSubvector(data.Z, threadId));
        // обчислення 2: а = а + а4 - КД1 - атомік-змінна
        Interlocked.Add(ref data.a, a4);

        // сигнал про завершення обчислення 2 - подія Е4
        data.E4.Set();
        // чекати, щоб всі потоки завершили обчислення 2 - події E1, Е2, Е3
        data.E1.WaitOne();
        data.E2.WaitOne();
        data.E3.WaitOne();

        // захист скаляру d за допомогою мютексу М1 - КД2
        data.M1.WaitOne();
        d4 = data.d;
        data.M1.ReleaseMutex();

        // захист скаляру а за допомогою критичної секції - КД3
        lock (data._lockObject)
        {
            a4 = data.a;
        }

        // обчислення 3: Xн = a4*(d4*Zн + R*(MO*MRн))
        data.CalculateStep3(a4, d4, threadId);

        // чекати, щоб всі потоки завершили обчислення 3 - семафор S1
        for (int i = 0; i < 3; i++)
        {
            data.S1.Wait();
        }

        stopwatch.Stop();

        // виведення результату Х
        Data.PrintVector(data.X);

        Console.WriteLine($"T{threadId} finish");
        Console.WriteLine($"Time taken: {stopwatch.ElapsedMilliseconds} ms");

        data.B1.Dispose();
    }
}
