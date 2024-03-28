package src;

import java.util.Arrays;

public class Thread2 extends Thread {
    private Data data;
    private int threadId;
    private int start;
    private int end;
    private int zi;
    private int di;

    public Thread2(int id, Data D) {
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
            // чекати, щоб інші потоки ввели дані - бар'єр B1
            data.B1.await();

            // обчислення 1
            int[] Z_H = Arrays.copyOfRange(data.Z, start, end);
            zi = Data.findMin(Z_H);

            // доступ до спільного ресурсу - КД1 - атомік-змінна z
            data.z.updateAndGet(current -> Math.min(current, zi));

            // сигнал про завершення обчислення 2 - семафор S2
            data.S2.release(3);
            // чекати, щоб всі потоки виконали обчислення 2 - семафори S1, S3, S4
            data.S1.acquire();
            data.S3.acquire();
            data.S4.acquire();

            // копіювання di = d - КД2 - семафор S6
            data.S6.acquire();
            di = data.d;
            data.S6.release();

            // копіювання zi = z - КД3 - критична секція CS1
            zi = data.CS1();

            // обчислення 3 і запис результату в MR
            data.calculateStep3(start, end, zi, di);

            // чекати, щоб інші потоки завершили обчислення 3 - семафор S5
            data.S5.acquire(3);

            // вивід MR як результату
            // Data.printMatrix(data.MR);
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
