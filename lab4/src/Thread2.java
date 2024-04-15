package src;

import java.util.Arrays;

public class Thread2 extends Thread {
    private Data data;
    private int threadId;
    private int start;
    private int end;
    private int[] S2;
    private int b2;
    private int e2;
    private int d2;

    public Thread2(int id, Data D) {
        data = D;
        threadId = id;
        start = (threadId - 1) * data.H;
        end = threadId * data.H;
    }

    @Override
    public void run() {
        System.out.println("T" + threadId + " start");
        try {
            data.fill_data_T2();
            
            // сигнал про введення даних і чекати, щоб інші потоки ввели дані
            data.signal_In();
            data.wait_In();

            // копіювання скаляру і обчислення 1
            d2 = data.copy_d();
            S2 = data.calculateStep1(start, end, d2);

            // обчислення 2
            data.put_S_sort(S2);

            // обчислення 3
            int[] B_H = Arrays.copyOfRange(data.B, start, end);
            b2 = Data.findMax(B_H);

            // обчислення 4
            data.put_b_max(b2);

            // сигнал про завершення обчислення 4 і чекати, щоб інші потоки теж його завершили
            data.signal_Calc4();
            data.wait_Calc4();

            // копіювання скалярів і обчислення 5
            b2 = data.copy_b();
            e2 = data.copy_e();
            data.calculateStep5(start, end, b2, e2);

            // сигнал про завершення всіх обчислень
            data.signal_Out();
        } catch (Exception e) {
            e.printStackTrace();
        } finally {
            System.out.println("T" + threadId + " finish");
        }
    }
}
