package src;

import java.util.Arrays;

public class Thread3 extends Thread {
    private Data data;
    private int threadId;
    private int start;
    private int end;
    private int[] S3;
    private int b3;
    private int e3;
    private int d3;

    public Thread3(int id, Data D) {
        data = D;
        threadId = id;
        start = (threadId - 1) * data.H;
        end = threadId * data.H;
    }

    @Override
    public void run() {
        System.out.println("T" + threadId + " start");
        try {
            // чекати, щоб інші потоки ввели дані
            data.wait_In();

            // копіювання скаляру і обчислення 1
            d3 = data.copy_d();
            S3 = data.calculateStep1(start, end, d3);

            // обчислення 2
            data.put_S_sort(S3);

            // обчислення 3
            int[] B_H = Arrays.copyOfRange(data.B, start, end);
            b3 = Data.findMax(B_H);

            // обчислення 4
            data.put_b_max(b3);

            // сигнал про завершення обчислення 4 і чекати, щоб інші потоки теж його завершили
            data.signal_Calc4();
            data.wait_Calc4();

            // копіювання скалярів і обчислення 5
            b3 = data.copy_b();
            e3 = data.copy_e();
            data.calculateStep5(start, end, b3, e3);

            // сигнал про завершення всіх обчислень
            data.signal_Out();
        } catch (Exception e) {
            e.printStackTrace();
        } finally {
            System.out.println("T" + threadId + " finish");
        }
    }
}
