package src;

import java.util.Arrays;

public class Thread3 extends Thread {
    private Data data;
    private int threadId;
    private int start;
    private int end;
    private int zi;
    private int di;

    public Thread3(int id, Data D) {
        data = D;
        threadId = id;
        start = (threadId - 1) * data.H;
        end = threadId * data.H;
    }

    private void fillData() {
        for (int i = 0; i < data.N; i++) {
            for (int j = 0; j < data.N; j++) {
                data.MC[i][j] = 1;
            }
        }
    }

    @Override
    public void run() {
        System.out.println("T" + threadId + " start");
        try {
            fillData();
            // сигнал про введення даних і чекати, щоб інші потоки ввели дані - бар'єр B1
            data.B1.await();

            // обчислення 1
            int[] Z_H = Arrays.copyOfRange(data.Z, start, end);
            zi = Data.findMin(Z_H);

            // доступ до спільного ресурсу - КД1 - атомік-змінна z
            data.z.updateAndGet(current -> Math.min(current, zi));

            // сигнал про завершення обчислення 2 - семафор S3
            data.S3.release(3);
            // чекати, щоб всі потоки виконали обчислення 2 - семафори S1, S2, S4
            data.S1.acquire();
            data.S2.acquire();
            data.S4.acquire();

            // копіювання di = d - КД2 - семафор S6
            data.S6.acquire();
            di = data.d;
            data.S6.release();

            // копіювання zi = z - КД3 - критична секція CS1
            zi = data.CS1();

            // обчислення 3 і запис результату в MR
            data.calculateStep3(start, end, zi, di);

            // сигнал про завершення обчислення 3 - семафор S5
            data.S5.release();
        } catch (Exception e) {
            e.printStackTrace();
        } finally {
            System.out.println("T" + threadId + " finish");
        }
    }
}
