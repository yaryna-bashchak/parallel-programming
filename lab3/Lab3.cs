using lab3;

class Lab3
{
    private static readonly Data data = new Data();
    // public static int N;
    // [DllImport("kernel32.dll")]
    // static extern IntPtr GetCurrentThread();

    // [DllImport("kernel32.dll")]
    // static extern IntPtr SetThreadAffinityMask(IntPtr hThread, IntPtr dwThreadAffinityMask);

    static void Main(string[] args)
    {
        Lab3 lab3Instance = new Lab3();

        var t1 = new Thread(new ParameterizedThreadStart(lab3Instance.Func1));
        var t2 = new Thread(new ParameterizedThreadStart(Func2));
        var t3 = new Thread(new ParameterizedThreadStart(Func3));
        var t4 = new Thread(new ParameterizedThreadStart(Func4));

        t1.Start(1);
        t2.Start(2);
        t3.Start(3);
        t4.Start(4);

        t1.Join();
        t2.Join();
        t3.Join();
        t4.Join();

        // Data.B1.Dispose();
    }

    void Func1(object param)
    {
        int threadId = (int)param;
        int a1, d1;
        Console.WriteLine($"T{threadId} start");

        // встановлення номера ядра
        // SetThreadAffinity(0);

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
        lock(data._lockObject)
        {
            a1 = data.a;
            Console.WriteLine(a1);
        }

        // обчислення 3: Xн = a1*(d1*Zн + R*(MO*MRн))
        data.CalculateStep3(a1, d1, threadId);

        // сигнал задачі Т4 про завершення обчислення 3 - семафор S1
        data.S1.Release();
        Console.WriteLine($"T{threadId} finish");
    }

    // static void SetThreadAffinity(int processorNumber)
    // {
    //     if (OperatingSystem.IsWindows())
    //     {
    //         IntPtr ptrThread = GetCurrentThread();
    //         SetThreadAffinityMask(ptrThread, new IntPtr(1 << processorNumber));
    //         Console.WriteLine($"Потік {Thread.CurrentThread.Name} виконується на ядрі {processorNumber}");
    //     }
    // }
}
