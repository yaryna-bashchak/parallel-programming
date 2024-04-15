package src;

import java.util.Arrays;

public class Thread4 extends Thread {
    private Data data;
    private int threadId;
    private int start;
    private int end;
    private int[] S4;
    private int b4;
    private int e4;
    private int d4;

    public Thread4(int id, Data D) {
        data = D;
        threadId = id;
        start = (threadId - 1) * data.H;
        end = threadId * data.H;
    }

    @Override
    public void run() {
        long startTime = System.currentTimeMillis();

        System.out.println("T" + threadId + " start");
        try {
            data.fill_data_T4();

            // сигнал про введення даних і чекати, щоб інші потоки ввели дані
            data.signal_In();
            data.wait_In();

            // копіювання скаляру і обчислення 1
            d4 = data.copy_d();
            S4 = data.calculateStep1(start, end, d4);

            // обчислення 2
            data.put_S_sort(S4);

            // обчислення 3
            int[] B_H = Arrays.copyOfRange(data.B, start, end);
            b4 = Data.findMax(B_H);

            // обчислення 4
            data.put_b_max(b4);

            // сигнал про завершення обчислення 4 і чекати, щоб інші потоки теж його завершили
            data.signal_Calc4();
            data.wait_Calc4();

            // копіювання скалярів і обчислення 5
            b4 = data.copy_b();
            e4 = data.copy_e();
            data.calculateStep5(start, end, b4, e4);

            // чекати, щоб інші потоки завершили всі обчислення
            data.wait_Out();

            // вивід результату
            Data.printVector(data.A);
        } catch (Exception e) {
            e.printStackTrace();
        } finally {
            System.out.println("T" + threadId + " finish");
            long endTime = System.currentTimeMillis();
            long totalTime = endTime - startTime;
            System.out.println("Time taken: " + totalTime + " ms");
        }
    }
}
