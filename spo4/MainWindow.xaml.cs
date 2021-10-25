using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Threading;

namespace spo4
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        private ThreadStart t1; //Делегат; представляет метод, выполняющийся в первом потоке
        private ThreadStart t2; //Делегат; представляет метод, выполняющийся во втором потоке
        private ThreadStart t3; //Делегат; представляет метод, выполняющийся в третьем потоке
        private Thread Thread_1; //Первый поток
        private Thread Thread_2; //Второй поток
        private Thread Thread_3; //Третий поток
        private Semaphore s; //Семафор, служащий для ограничения числа потоков, которые обращаются к richText
        private bool first; //Переменная, хранящая информацию первый ли запуск потоков
        private int pause; //Переменная, хранящая длительность паузы

        public MainWindow()
        {
            InitializeComponent();
            s = new Semaphore(1, 1);
            first = true;
            pause = 100;
        }
        private void TreadHelper(int fNum, ulong i) => this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
        {
            switch (fNum)
            {
                case 1:
                    if (i == 0)
                        listBox1.Items.Clear();
                    else
                        listBox1.Items.Add(i);
                    break;
                case 2:
                    if (i == 0)
                        listBox2.Items.Clear();
                    else
                        listBox2.Items.Add(i);
                    break;
                case 3:
                    if (i == 0)
                        listBox3.Items.Clear();
                    else
                        listBox3.Items.Add(i);
                    break;
                default:
                    break;
            }

        });

        private void SwitchHelper(int fNum, bool value, string text) => this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
        {
            switch (fNum)
            {
                case 1:
                    FirstThread.IsEnabled = value;
                    FirstThread.Content = text;
                    break;
                case 2:
                    SecondThread.IsEnabled = value;
                    SecondThread.Content = text;
                    break;
                case 3:
                    ThirdThread.IsEnabled = value;
                    ThirdThread.Content = text;
                    break;
                default:
                    break;
            }

        });

        private void f1(int n1)
        {
            TreadHelper(1, 0);
            List<ulong> arr_1 = new List<ulong>();
            ulong i = 2;
            while (arr_1.Count < n1)
            {
                if (arr_1.Count == 0)
                {
                    arr_1.Add(2);
                    TreadHelper(1, i);
                    s.WaitOne(); //Блокируем текущий поток до освобождения семафора
                    writeTextBox(1); //Записываем в richText номер потока
                    s.Release(1); //Освобождаем семафор
                    Thread.Sleep(pause); //Блокируем текущий поток на заданное количество миллисекунд
                    i = 3;
                    continue;
                }
                bool temp = false;
                for (int k = 0; k < arr_1.Count; k++)
                {
                    if (i % arr_1[k] == 0)
                    {
                        temp = true;
                    }
                }
                if (!temp)
                {
                    arr_1.Add(i);
                    TreadHelper(1, i);
                    _ = s.WaitOne(); //Блокируем текущий поток до освобождения семафора
                    writeTextBox(1); //Записываем в richText номер потока
                    _ = s.Release(1); //Освобождаем семафор
                    Thread.Sleep(pause); //Блокируем текущий поток на заданное количество миллисекунд
                }
                i++;
            }
            SwitchHelper(1, false, "Поток №1 не запущен");
        }

        private void f2(int n2)
        {
            TreadHelper(2, 0);
            List<ulong> arr_2 = new List<ulong>();
            for (int i = 0; i < n2; i++)
            {
                if (arr_2.Count == 0 || arr_2.Count == 1)
                {
                    arr_2.Add(1);
                    TreadHelper(2, Convert.ToUInt64(i));
                    s.WaitOne(); //Блокируем текущий поток до освобождения семафора
                    writeTextBox(2); //Записываем в richText номер потока
                    s.Release(1); //Освобождаем семафор
                    Thread.Sleep(pause); //Блокируем текущий поток на заданное количество миллисекунд
                    continue;
                }
                else
                {
                    arr_2.Add(arr_2[arr_2.Count - 1] + arr_2[arr_2.Count - 2]);
                    TreadHelper(2, Convert.ToUInt64(i));
                    s.WaitOne(); //Блокируем текущий поток до освобождения семафора
                    writeTextBox(2); //Записываем в richText номер потока
                    s.Release(1); //Освобождаем семафор
                    Thread.Sleep(pause); //Блокируем текущий поток на заданное количество миллисекунд
                }
            }
            SwitchHelper(2, false, "Поток №2 не запущен");
        }

        private void f3(int n3)
        {
            TreadHelper(3, 0);
            List<ulong> arr_3 = new List<ulong>();
            for (int i = 1; i <= n3; i++)
            {
                if (arr_3.Count == 0)
                {
                    arr_3.Add((ulong)i);
                    TreadHelper(3, arr_3[i - 1]);
                    s.WaitOne(); //Блокируем текущий поток до освобождения семафора
                    writeTextBox(3); //Записываем в richText номер потока
                    s.Release(1); //Освобождаем семафор
                    Thread.Sleep(pause); //Блокируем текущий поток на заданное количество миллисекунд
                }
                else
                {
                    arr_3.Add(arr_3[arr_3.Count - 1] * (ulong)i);
                    TreadHelper(3, arr_3[i - 1]);
                    s.WaitOne(); //Блокируем текущий поток до освобождения семафора
                    writeTextBox(3); //Записываем в richText номер потока
                    s.Release(1); //Освобождаем семафор
                    Thread.Sleep(pause); //Блокируем текущий поток на заданное количество миллисекунд
                }
            }
            SwitchHelper(3, false, "Поток №3 не запущен");
        }

        public void writeTextBox(int r) => this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
        {
            switch (r)
            {
                case 1:
                    richText.AppendText("[" + r.ToString(), "#bc642e");
                    break;
                case 2:
                    richText.AppendText("[" + r.ToString(), "#5584a5");
                    break;
                case 3:
                    richText.AppendText("[" + r.ToString(), "#dcc06c");
                    break;
            }
            richText.AppendText("] ");
        });

        private void StarThread_Click(object sender, RoutedEventArgs e)
        {
            richText.Document.Blocks.Clear();
            int n1;
            try
            {
                n1 = Int32.Parse(textBox1.Text); //Считываем параметр для расчета решета Эрастовена
            }
            catch
            {
                n1 = 0;
                textBox1.Text = n1.ToString();
            }
            int n2;
            try
            {
                n2 = Int32.Parse(textBox2.Text); //Считываем параметр для расчета чисел Фибоначчи
            }
            catch
            {
                n2 = 0;
                textBox2.Text = n2.ToString();
            }
            int n3;
            try
            {
                n3 = Int32.Parse(textBox3.Text); //Считываем параметр для расчета факториала
            }
            catch
            {
                n3 = 0;
                textBox3.Text = n3.ToString();
            }
            if (!first) //Если не первый запуск потоков
            {
                s.WaitOne(); //Ждем свободный семафор
                Thread_1.Abort(); //Завершаем поток
                Thread_2.Abort(); //Завершаем поток
                Thread_3.Abort(); //Завершаем поток
            }
            t1 = delegate { f1(n1); }; //Связываем делегат t1 с функцией f1
            t2 = delegate { f2(n2); }; //Связываем делегат t2 с функцией f2
            t3 = delegate { f3(n3); }; //Связываем делегат t3 с функцией f3
            s = new Semaphore(1, 1); //Создание семафора с начальным и максимальным количеством запросов для семафора, которое может быть обеспечено одновременно
            Thread_1 = new Thread(t1); //Создаем первый поток
            Thread_2 = new Thread(t2); //Создаем второй поток
            Thread_3 = new Thread(t3); //Создаем третий поток
            Thread_1.Name = "Thread1"; //Указываем имя для потока
            Thread_2.Name = "Thread2"; //Указываем имя для потока
            Thread_3.Name = "Thread3"; //Указываем имя для потока
            Thread_1.Priority = (ThreadPriority)Enum.Parse(typeof(ThreadPriority), Convert.ToString((int)Slider1.Value - 1));//Устанавливаем приоритет для потока
            Thread_2.Priority = (ThreadPriority)Enum.Parse(typeof(ThreadPriority), Convert.ToString((int)Slider2.Value - 1));//Устанавливаем приоритет для потока
            Thread_3.Priority = (ThreadPriority)Enum.Parse(typeof(ThreadPriority), Convert.ToString((int)Slider3.Value - 1));//Устанавливаем приоритет для потока
            Thread_1.Start(); //Запускаем поток
            Thread_2.Start(); //Запускаем поток
            Thread_3.Start(); //Запускаем поток
            SwitchHelper(1, Thread_1.IsAlive, "Остановить поток №1");
            SwitchHelper(2, Thread_2.IsAlive, "Остановить поток №2");
            SwitchHelper(3, Thread_3.IsAlive, "Остановить поток №3");
        }

        private void FirstThread_Click(object sender, RoutedEventArgs e)
        {
            if (!Thread_1.IsAlive)
            {
                _ = s.WaitOne();
                Thread_1 = new Thread(t1);
                Thread_1.Name = "Thread1";
                Thread_1.Start();
                Thread_1.Priority = (ThreadPriority)Enum.Parse(typeof(ThreadPriority), Convert.ToString(Slider1.Value - 1));
                FirstThread.Content = "Остановить поток №1";
                _ = s.Release(1);
            }
            else
            {
                _ = s.WaitOne();
                Thread_1.Abort();
                FirstThread.Content = "Начать заново поток №1";
                _ = s.Release(1);
            }
        }

        private void SecondThread_Click(object sender, RoutedEventArgs e)
        {
            if (!Thread_2.IsAlive)
            {
                _ = s.WaitOne();
                Thread_2 = new Thread(t2);
                Thread_2.Name = "Thread2";
                Thread_2.Start();
                Thread_2.Priority = (ThreadPriority)Enum.Parse(typeof(ThreadPriority), Convert.ToString(Slider2.Value - 1));
                SecondThread.Content = "Остановить поток №2";
                _ = s.Release(1);
            }
            else
            {
                _ = s.WaitOne();
                Thread_2.Abort();
                SecondThread.Content = "Начать заново поток №2";
                _ = s.Release(1);
            }
        }

        private void ThirdThread_Click(object sender, RoutedEventArgs e)
        {
            if (!Thread_3.IsAlive)
            {
                _ = s.WaitOne();
                Thread_3 = new Thread(t3);
                Thread_3.Name = "Thread3";
                Thread_3.Start();
                Thread_3.Priority = (ThreadPriority)Enum.Parse(typeof(ThreadPriority), Convert.ToString(Slider3.Value - 1));
                ThirdThread.Content = "Остановить поток №3";
                _ = s.Release(1);
            }
            else
            {
                _ = s.WaitOne();
                Thread_3.Abort();
                ThirdThread.Content = "Начать заново поток №3";
                _ = s.Release(1);
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            _ = s.WaitOne();
            Environment.Exit(1);
        }

        private void Slider1_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (Thread_1 != null && Thread_1.IsAlive)
            {
                Thread_1.Priority = (ThreadPriority)Enum.Parse(typeof(ThreadPriority), Convert.ToString(Slider1.Value - 1));
            }
            ((Slider)sender).SelectionEnd = e.NewValue;
        }

        private void Slider2_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (Thread_2 != null && Thread_2.IsAlive)
            {
                Thread_2.Priority = (ThreadPriority)Enum.Parse(typeof(ThreadPriority), Convert.ToString(Slider2.Value - 1));
            }
            ((Slider)sender).SelectionEnd = e.NewValue;
        }

        private void Slider3_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (Thread_3 != null && Thread_3.IsAlive)
            {
                Thread_3.Priority = (ThreadPriority)Enum.Parse(typeof(ThreadPriority), Convert.ToString(Slider3.Value - 1));
            }
            ((Slider)sender).SelectionEnd = e.NewValue;
        }

        private void SliderPause_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            pause = (int)SliderPause.Value * 100;
            ((Slider)sender).SelectionEnd = e.NewValue;
        }

        //private void TextChanged(object sender, TextChangedEventArgs e)
        //{

        //}
    }

    public static class TextExtension
    {
        public static void AppendText(this RichTextBox box, string text, string color)
        {
            BrushConverter bc = new BrushConverter();
            TextRange tr = new TextRange(box.Document.ContentEnd, box.Document.ContentEnd);
            tr.Text = text;
            try
            {
                tr.ApplyPropertyValue(TextElement.ForegroundProperty,
                    bc.ConvertFromString(color));
            }
            catch (FormatException) { }
        }
    }
}