package src;

import java.util.Arrays;

public class Data {
    public int N = 2000;
    public int P = 4;
    public int H = N / P;
    public int[][] MM = new int[N][N];
    public int[][] MX = new int[N][N];
    public int[] A = new int[N];
    public int[] B = new int[N];
    public int[] Z = new int[N];

    private int[] S = new int[N];
    private int b = Integer.MIN_VALUE;
    private int e;
    private int d;

    private int Fl1 = 0;
    private int Fl2 = 0;
    private int Fl3 = 0;

    public void fill_data_T1() {
        d = 1;
        for (int i = 0; i < N; i++) {
            for (int j = 0; j < N; j++) {
                MM[i][j] = 1;
            }
        }
    }

    public void fill_data_T2() {
        e = 1;
        for (int i = 0; i < N; i++) {
            B[i] = 1;
            for (int j = 0; j < N; j++) {
                MX[i][j] = 1;
            }
        }
        // B[2] = 10;
        // B[5] = 5;
    }

    public void fill_data_T4() {
        for (int i = 0; i < N; i++) {
            Z[i] = 1;
        }
    }

    public synchronized int copy_b() {
        return b;
    }

    public synchronized int copy_e() {
        return e;
    }

    public synchronized int copy_d() {
        return d;
    }

    public synchronized void put_b_max(int bi) {
        if (bi > b) {
            b = bi;
        }
    }

    public synchronized void put_S_sort(int[] new_Si) {
        int filled = 0;
        for (int value : S) {
            if (value != 0) {
                filled++;
            } else {
                break;
            }
        }

        int[] temp = new int[filled + new_Si.length];
        int i = 0, j = 0, k = 0;

        while (i < filled && j < new_Si.length) {
            if (S[i] < new_Si[j]) {
                temp[k++] = S[i++];
            } else {
                temp[k++] = new_Si[j++];
            }
        }

        while (i < filled) {
            temp[k++] = S[i++];
        }

        while (j < new_Si.length) {
            temp[k++] = new_Si[j++];
        }

        System.arraycopy(temp, 0, S, 0, temp.length);
    }

    public synchronized void wait_In() {
        try {
            if (Fl1 < 3) {
                wait();
            }
        } catch (Exception e) {
        }
    }

    public synchronized void wait_Calc4() {
        try {
            if (Fl2 < 4) {
                wait();
            }
        } catch (Exception e) {
        }
    }

    public synchronized void wait_Out() {
        try {
            if (Fl3 < 3) {
                wait();
            }
        } catch (Exception e) {
        }
    }

    public synchronized void signal_In() {
        Fl1++;
        if (Fl1 == 3) {
            notifyAll();
        }
    }

    public synchronized void signal_Calc4() {
        Fl2++;
        if (Fl2 == 4) {
            notifyAll();
        }
    }

    public synchronized void signal_Out() {
        Fl3++;
        if (Fl3 == 3) {
            notify();
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

    public static int[] multiplyVectorByNumber(int[] vector, int number) {
        int size = vector.length;
        int[] result = new int[size];

        for (int i = 0; i < size; i++) {
            result[i] = vector[i] * number;
        }

        return result;
    }

    public static int[] addVectors(int[] vector1, int[] vector2) {
        int size = vector1.length;
        int[] result = new int[size];

        for (int i = 0; i < size; i++) {
            result[i] = vector1[i] + vector2[i];
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

    public int[] calculateStep1(int start, int end, int di) {
        int[] B_H = Arrays.copyOfRange(B, start, end);
        int[][] MX_H = getSubmatrixFromColumns(MX, start, end);
        int[][] MM_MX_H = multiplyMatrices(MM, MX_H);

        int[] Si = addVectors(multiplyVectorByNumber(B_H, di), multiplyVectorByMatrix(Z, MM_MX_H));
        Arrays.sort(Si);

        return Si;
    }

    public void calculateStep5(int start, int end, int bi, int ei) {
        int[] S_H = Arrays.copyOfRange(S, start, end);
        int[] Z_H = Arrays.copyOfRange(Z, start, end);

        int[] A_H = addVectors(multiplyVectorByNumber(S_H, ei), multiplyVectorByNumber(Z_H, bi));

        insertSubvectorIntoVector(A, A_H, start);
    }

    public static void printVector(int[] vector) {
        for (int i = 0; i < vector.length; i++) {
            System.out.print(vector[i] + " ");
        }
        System.out.println();
    }
}