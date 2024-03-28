// Програмне забезпечення високопродуктивних комп'ютерних систем
// Лабораторна робота №1: програмування потоків, потоки в мові С#
// номер в списку групи 3: 1.24  2.19  3.15
// F1: E = A + C *(MA*ME) + B
// F2: k = MAX(MF + MG*ML)
// F3: S = (O+P)*TRANS(MR * MT)
// Бащак Ярина Володимирівна
// група ІМ-11
// 16.02.2024

using System.Runtime.InteropServices;
using System.Text;
using lab1;

class Lab1
{
    public static int N;
    [DllImport("kernel32.dll")]
    static extern IntPtr GetCurrentThread();

    [DllImport("kernel32.dll")]
    static extern IntPtr SetThreadAffinityMask(IntPtr hThread, IntPtr dwThreadAffinityMask);

    static void Main(string[] args)
    {
        Console.WriteLine("Введіть значення N:");
        while (!int.TryParse(Console.ReadLine(), out N) || N <= 0)
        {
            Console.WriteLine("Будь ласка, введіть коректне ціле число більше 0:");
        }

        int stackSize = 1024 * 1024;

        var t1 = new Thread(Func1, stackSize)
        {
            Name = "T1",
            Priority = ThreadPriority.Normal
        };

        var t2 = new Thread(Func2, stackSize)
        {
            Name = "T2",
            Priority = ThreadPriority.Normal
        };

        var t3 = new Thread(Func3, stackSize)
        {
            Name = "T3",
            Priority = ThreadPriority.Normal
        };

        t1.Start();
        t2.Start();
        t3.Start();
    }

    static void SetThreadAffinity(int processorNumber)
    {
        if (OperatingSystem.IsWindows())
        {
            IntPtr ptrThread = GetCurrentThread();
            SetThreadAffinityMask(ptrThread, new IntPtr(1 << processorNumber));
            Console.WriteLine($"Потік {Thread.CurrentThread.Name} виконується на ядрі {processorNumber}");
        }
    }

    static void Func1()
    {
        Console.WriteLine("T1 start");

        // встановлення номера ядра
        SetThreadAffinity(0);

        var data = new Data(N);
        data.InitializeForFunc1();
        var result = data.F1();

        var output = new StringBuilder();
        output.Append("T1 result: ");
        foreach (var item in result)
        {
            output.Append(item + " ");
        }

        data.OutputResults(output.ToString(), "results_func1.txt");
        Console.WriteLine("T1 finish");
    }

    static void Func2()
    {
        Console.WriteLine("T2 start");

        // встановлення номера ядра
        SetThreadAffinity(1);

        var data = new Data(N);
        data.InitializeForFunc2();
        var result = data.F2();

        var output = "T2 result: " + result;

        data.OutputResults(output, "results_func2.txt");
        Console.WriteLine("T2 finish");
    }

    static void Func3()
    {
        Console.WriteLine("T3 start");
        // встановлення номера ядра
        SetThreadAffinity(2);

        var data = new Data(N);
        data.InitializeForFunc3();
        var result = data.F3();

        var output = new StringBuilder();
        output.Append("T3 result: ");
        foreach (var item in result)
        {
            output.Append(item + " ");
        }

        data.OutputResults(output.ToString(), "results_func3.txt");
        Console.WriteLine("T3 finish");
    }
}
