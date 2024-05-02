package src;

import java.util.concurrent.CyclicBarrier;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;

public class Data {
    public int N = 1000;
    public int P = 4;
    public int H = N / P;
    public int[][] MA = new int[N][N];
    public int[][] MD = new int[N][N];
    public int[] B = new int[N];
    public int[] C = new int[N];
    public int[] R = new int[N];
    public int[] X = new int[N];
    public int p;
    public int e;
    public int a = Integer.MIN_VALUE;
    CyclicBarrier barrier = new CyclicBarrier(P);
    public Object calc2 = new Object();
    public Object copy_p = new Object();
    public Object copy_a = new Object();
    public Object copy_e = new Object();

    ExecutorService executor = Executors.newFixedThreadPool(P);

    public void fill_data_T1() {
        e = 1;
    }

    public void fill_data_T2() {
        for (int i = 0; i < N; i++) {
            C[i] = 1;
            for (int j = 0; j < N; j++) {
                MA[i][j] = 1;
            }
        }
    }

    public void fill_data_T3() {
        for (int i = 0; i < N; i++) {
            R[i] = 1;
            for (int j = 0; j < N; j++) {
                MD[i][j] = 1;
            }
        }
        // MD[1][2] = 10;
    }

    public void fill_data_T4() {
        p = 1;
        for (int i = 0; i < N; i++) {
            B[i] = 1;
        }
    }

    public static int findMax(int[] vector) {
        int max = vector[0];
        for (int i = 0; i < vector.length; i++) {
            if (vector[i] > max) {
                max = vector[i];
            }
        }
        return max;
    }

    public static int[][] multiplyMatrices(int[][] matrix1, int[][] matrix2) {
        int m = matrix1.length;
        int n = matrix2.length;
        int p = matrix2[0].length;

        int[][] result = new int[m][p];

        for (int i = 0; i < m; i++) {
            for (int j = 0; j < p; j++) {
                result[i][j] = 0;
                for (int k = 0; k < n; k++) {
                    result[i][j] += matrix1[i][k] * matrix2[k][j];
                }
            }
        }
        return result;
    }

    public static int[] multiplyVectorByMatrix(int[] vector, int[][] matrix) {
        int n = vector.length;
        int m = matrix[0].length;

        int[] result = new int[m];

        for (int i = 0; i < m; i++) {
            result[i] = 0;
            for (int j = 0; j < n; j++) {
                result[i] += vector[j] * matrix[j][i];
            }
        }

        return result;
    }

    public static int[][] getSubmatrixFromColumns(int[][] matrix, int start, int end) {
        int rows = matrix.length;
        int columns = end - start;

        int[][] submatrix = new int[rows][columns];

        for (int i = 0; i < rows; i++) {
            for (int j = start; j < end; j++) {
                submatrix[i][j - start] = matrix[i][j];
            }
        }

        return submatrix;
    }

    public static void insertSubvectorIntoVector(int[] targetVector, int[] subvector, int start) {
        int submatrixSize = subvector.length;
        for (int i = 0; i < submatrixSize; i++) {
            targetVector[start + i] = subvector[i];
        }
    }

    public static void printVector(int[] vector) {
        for (int i = 0; i < vector.length; i++) {
            System.out.print(vector[i] + " ");
        }
        System.out.println();
    }
}