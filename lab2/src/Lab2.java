// Програмне забезпечення високопродуктивних комп'ютерних систем
// Лабораторна робота №2: програмування для КС із СП. Семафори, критичні секції, атомік-змінні, бар’єри
// варіант 10
// MR = MB*(MC*MM)*d + min(Z)*MC
// Бащак Ярина Володимирівна
// група ІМ-11
// 20.03.2024

package src;
// javac -d bin src/Lab2.java
// java -cp bin src.Lab2
// taskset -c 0 java -cp bin src.Lab2

public class Lab2 {
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
