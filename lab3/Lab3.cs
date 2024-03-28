using lab3;

class Lab3
{
    // public static int N;
    // [DllImport("kernel32.dll")]
    // static extern IntPtr GetCurrentThread();

    // [DllImport("kernel32.dll")]
    // static extern IntPtr SetThreadAffinityMask(IntPtr hThread, IntPtr dwThreadAffinityMask);

    static void Main(string[] args)
    {
        var t1 = new Thread(new ThreadStart(Func1));
        var t2 = new Thread(new ThreadStart(Func2));
        var t3 = new Thread(new ThreadStart(Func3));
        var t4 = new Thread(new ThreadStart(Func4));

        t1.Start();
        t2.Start();
        t3.Start();
        t4.Start();

        t1.Join();
        t2.Join();
        t3.Join();
        t4.Join();
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

    static void Func1()
    {
        Console.WriteLine("T1 start");

        // встановлення номера ядра
        // SetThreadAffinity(0);

        var data = new Data(N);
        data.InitializeForFunc1();
        var result = data.F1();

        var output = new StringBuilder();
        output.Append("T1 result: ");
        foreach (var item in result)
        {
            output.Append(item + " ")
        }

        data.OutputResults(output.ToString(), "results_func1.txt");
        Console.WriteLine("T1 finish");
    }
}
