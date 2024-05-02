// Програмне забезпечення високопродуктивних комп'ютерних систем
// Лабораторна робота №5: OpenMP
// варіант 8
// X = p*max(C*(MA*MD))*R+ e*B
// Бащак Ярина Володимирівна
// група ІМ-11
// 02.05.2024

package src;
// javac -d bin src/Lab5.java

// java -cp bin src.Lab5
// taskset -c 0 java -cp bin src.Lab5

import java.util.concurrent.BrokenBarrierException;
import java.util.concurrent.TimeUnit;
import java.util.stream.IntStream;

public class Lab5 {
    public static void main(String[] args) {
        Data data = new Data();
        final long startTime = System.nanoTime();

        for (int i = 0; i < data.P; i++) {
            final int start = i * data.H;
            int end = (i + 1) * data.H;
            int threadId = i + 1;

            data.executor.submit(() -> {
                int ai, pi, ei;

                System.out.println("Thread " + threadId + " start");

                // введення даних
                if (threadId == 1) {
                    data.fill_data_T1();
                } else if (threadId == 2) {
                    data.fill_data_T2();
                } else if (threadId == 3) {
                    data.fill_data_T3();
                } else {
                    data.fill_data_T4();
                }

                // бар'єр
                try {
                    data.barrier.await();
                } catch (InterruptedException | BrokenBarrierException e) {
                    e.printStackTrace();
                }

                // обчислення 1
                int[][] MD_H = Data.getSubmatrixFromColumns(data.MD, start, end);
                ai = Data.findMax(Data.multiplyVectorByMatrix(data.C, Data.multiplyMatrices(data.MA, MD_H)));

                // обчислення 2
                synchronized (data.calc2) {
                    if (ai > data.a) {
                        data.a = ai;
                    }
                }

                // бар'єр
                try {
                    data.barrier.await();
                } catch (InterruptedException | BrokenBarrierException e) {
                    e.printStackTrace();
                }

                // копіювання p, a, e
                synchronized (data.copy_p) {
                    pi = data.p;
                }

                synchronized (data.copy_a) {
                    ai = data.a;
                }

                synchronized (data.copy_e) {
                    ei = data.e;
                }

                // обчислення 3 - імітація поведінки #pragma omp parallel for
                final int finalAi = ai;
                IntStream.range(start, end).parallel().forEach(index -> {
                    data.X[index] = pi * finalAi * data.R[index] + ei * data.B[index];
                });

                // бар'єр
                try {
                    data.barrier.await();
                } catch (InterruptedException | BrokenBarrierException e) {
                    e.printStackTrace();
                }

                // потік 1 виводить вектор Х
                if (threadId == 1) {
                    Data.printVector(data.X);
                }

                System.out.println("Thread " + threadId + " finish");
            });
        }

        data.executor.shutdown();

        try {
            if (!data.executor.awaitTermination(1, TimeUnit.HOURS)) {
                System.out.println("Executor did not terminate in the specified time.");
            }
            long endTime = System.nanoTime();
            long totalExecutionTime = (endTime - startTime) / 1_000_000;
            System.out.println("Time taken: " + totalExecutionTime + " ms");
        } catch (InterruptedException e) {
            System.out.println("Threads interrupted: " + e.getMessage());
        }
    }
}
