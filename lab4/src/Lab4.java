// Програмне забезпечення високопродуктивних комп'ютерних систем
// Лабораторна робота №4: монітори
// варіант 12
// A = sort(d*B + Z*(MM*MX))*e + max(B)*Z
// Бащак Ярина Володимирівна
// група ІМ-11
// 15.04.2024

package src;
// javac -d bin src/Lab4.java
// java -cp bin src.Lab4
// taskset -c 0 java -cp bin src.Lab4

public class Lab4 {
    public static void main(String[] args) {
        Data D = new Data();

        Thread1 T1 = new Thread1(1, D);
        Thread2 T2 = new Thread2(2, D);
        Thread3 T3 = new Thread3(3, D);
        Thread4 T4 = new Thread4(4, D);

        T1.start();
        T2.start();
        T3.start();
        T4.start();
    }
}
