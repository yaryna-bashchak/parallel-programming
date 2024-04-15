package src;

import java.util.Arrays;

public class Thread1 extends Thread {
    private Data data;
    private int threadId;
    private int start;
    private int end;
    private int[] S1;
    private int b1;
    private int e1;
    private int d1;

    public Thread1(int id, Data D) {
        data = D;
        threadId = id;
        start = (threadId - 1) * data.H;
        end = threadId * data.H;
    }

    @Override
    public void run() {
        System.out.println("T" + threadId + " start");
        try {
            data.fill_data_T1();
            
            // сигнал про введення даних і чекати, щоб інші потоки ввели дані
            data.signal_In();
            data.wait_In();

            // копіювання скаляру і обчислення 1
            d1 = data.copy_d();
            S1 = data.calculateStep1(start, end, d1);

            // обчислення 2
            data.put_S_sort(S1);

            // обчислення 3
            int[] B_H = Arrays.copyOfRange(data.B, start, end);
            b1 = Data.findMax(B_H);

            // обчислення 4
            data.put_b_max(b1);

            // сигнал про завершення обчислення 4 і чекати, щоб інші потоки теж його завершили
            data.signal_Calc4();
            data.wait_Calc4();

            // копіювання скалярів і обчислення 5
            b1 = data.copy_b();
            e1 = data.copy_e();
            data.calculateStep5(start, end, b1, e1);

            // сигнал про завершення всіх обчислень
            data.signal_Out();
        } catch (Exception e) {
            e.printStackTrace();
        } finally {
            System.out.println("T" + threadId + " finish");
        }
    }
}
