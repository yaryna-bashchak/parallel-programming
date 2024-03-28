package src;

import java.util.concurrent.CyclicBarrier;
import java.util.concurrent.Semaphore;
import java.util.concurrent.atomic.AtomicInteger;

public class Data {
    public int N = 2000;
    public int P = 4;
    public int H = N / P;
    public int[][] MB = new int[N][N];
    public int[][] MC = new int[N][N];
    public int[] Z = new int[N];
    public AtomicInteger z = new AtomicInteger(Integer.MAX_VALUE);
    public int d;
    public int[][] MM = new int[N][N];
    public int[][] MR = new int[N][N];

    Semaphore S1 = new Semaphore(0);
    Semaphore S2 = new Semaphore(0);
    Semaphore S3 = new Semaphore(0);
    Semaphore S4 = new Semaphore(0);
    Semaphore S5 = new Semaphore(0);
    Semaphore S6 = new Semaphore(1);

    CyclicBarrier B1 = new CyclicBarrier(4);

    public synchronized int CS1() {
        return z.intValue();
    }

    public static int findMin(int[] vector) {
        int min = vector[0];
        for (int i = 0; i < vector.length; i++) {
            if (vector[i] < min) {
                min = vector[i];
            }
        }
        return min;
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

    public static int[][] multiplyMatrixByNumber(int[][] matrix, int number) {
        int rows = matrix.length;
        int columns = matrix[0].length;
        int[][] result = new int[rows][columns];

        for (int i = 0; i < rows; i++) {
            for (int j = 0; j < columns; j++) {
                result[i][j] = matrix[i][j] * number;
            }
        }

        return result;
    }

    public static int[][] addMatrices(int[][] matrix1, int[][] matrix2) {
        int rows = matrix1.length;
        int columns = matrix1[0].length;
        int[][] result = new int[rows][columns];

        for (int i = 0; i < rows; i++) {
            for (int j = 0; j < columns; j++) {
                result[i][j] = matrix1[i][j] + matrix2[i][j];
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

    public static void insertSubmatrixIntoMatrix(int[][] targetMatrix, int[][] submatrix, int start) {
        int rows = targetMatrix.length;
        int submatrixColumns = submatrix[0].length;

        for (int i = 0; i < rows; i++) {
            for (int j = 0; j < submatrixColumns; j++) {
                targetMatrix[i][start + j] = submatrix[i][j];
            }
        }
    }

    public void calculateStep3(int start, int end, int zi, int di) {
        int[][] MM_H = Data.getSubmatrixFromColumns(MM, start, end);
        int[][] MC_H = Data.getSubmatrixFromColumns(MC, start, end);

        int[][] MC_MMh = Data.multiplyMatrices(MC, MM_H);
        int[][] MB_MC_MMh = Data.multiplyMatrices(MB, MC_MMh);
        int[][] MB_MC_MMh_d = Data.multiplyMatrixByNumber(MB_MC_MMh, di);
        int[][] z_MCh = Data.multiplyMatrixByNumber(MC_H, zi);

        int[][] MR_H = Data.addMatrices(MB_MC_MMh_d, z_MCh);
        Data.insertSubmatrixIntoMatrix(MR, MR_H, start);
    }

    public static void printMatrix(int[][] matrix) {
        for (int i = 0; i < matrix.length; i++) {
            for (int j = 0; j < matrix[i].length; j++) {
                System.out.print(matrix[i][j] + " ");
            }
            System.out.println();
        }
    }
}