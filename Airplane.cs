using Flight_Simulator;
using System;
using System.Collections.Generic;
using System.Data;
using System.Security.Cryptography;
using static System.Console;
using System.Threading;
using System.IO;
using System.Text;

namespace Flight_Simulator
{

    class Airplane
    {

        private int currentSpeed;
        private int currentHeight;
        private int totalPenalty;
        private bool IsSpeedGained;
        private bool IsFlyBegin;

        private delegate void ChangeDelegate(int speed, int height);
        private event ChangeDelegate ChangeEvent;

        public Airplane()
        {

            currentSpeed = 0;
            currentHeight = 0;
            totalPenalty = 0;
            IsSpeedGained = false;//Флаг набора максимальной скорости
            IsFlyBegin = false;//Флаг начала полета
        }

        public void Record(string str)
        {
            string fPath = "Record.txt";
            using (FileStream fs = new FileStream(fPath, FileMode.Append))
            {
                using (StreamWriter sw = new StreamWriter(fs, Encoding.Unicode))
                {
                    sw.WriteLine(str);
                }
            }
        }

        public void Fly()
        {
            WriteLine("Управление:\n\nRightArrow - увеличить скорость самолета на 50,\nLeftArrow - уменьшить скорость самолета на 50,\nShift + D - увеличить скорость самолета на 150,\nShift + A - уменьшить скорость самолета на 150,\n\nUpArrow - увеличить высоту самолета на 250,\nDownArrow - уменьшить высоту самолета на 250,\nShift + W - увеличить высоту самолета на 500,\nShift + S - уменьшить высоту самолета на 500.\n");
            WriteLine("Задача пилота — взлететь на самолете, набрать максимальную (1000 км/ч.) скорость, \nа затем посадить самолет.");

            ConsoleKeyInfo key;

            Console.Write($"\nВведите первого имя диспетчера: ");
            Dispatcher d1 = new Dispatcher(ReadLine());
            Console.Write($"\nВведите имя второго диспетчера: ");
            Dispatcher d2 = new Dispatcher(ReadLine());
            Clear();
            WriteLine("\t\t\t\t\tНачинайте полет");
            Thread.Sleep(1000);
            int distance = 0;
            string str = "";
            //int x = 10, y = 40;
            while (true)
            {

                key = Console.ReadKey();

                if ((key.Modifiers & ConsoleModifiers.Shift) != 0)
                {
                    if (key.Key == ConsoleKey.D) currentSpeed += 150;
                    else if (key.Key == ConsoleKey.A) currentSpeed -= 150;
                    else if (key.Key == ConsoleKey.W) currentHeight += 500;
                    else if (key.Key == ConsoleKey.S) currentHeight -= 500;
                }
                else
                {
                    if (key.Key == ConsoleKey.RightArrow) currentSpeed += 50;
                    else if (key.Key == ConsoleKey.LeftArrow) currentSpeed -= 50;
                    else if (key.Key == ConsoleKey.UpArrow) currentHeight += 250;
                    else if (key.Key == ConsoleKey.DownArrow) currentHeight -= 250;

                }

                if (currentSpeed < 50) WriteLine("Набирайте скорость!");

                if (currentSpeed >= 50)
                {
                    if (distance <= 500)
                    {
                        ChangeEvent += d1.RecomendedHight;
                        Clear();
                        if (!IsFlyBegin)// Оповещение о начале полета
                        {
                            
                            System.Media.SystemSounds.Hand.Play();
                            WriteLine("\t\t\t\t\tПолет начался!\n\n");

                            WriteLine(@"          
             ______
            _\ _~-\___
    =  = ==(____AA____D
                \_____\___________________,-~~~~~~~`-.._
                /     o O o o o o O O o o o o o o O o  |\_
                `~-.__        ___..----..                  )
                      `---~~\___________/------------`````
                      =  ===(_________D");
                            Thread.Sleep(1500);
                        }

                        IsFlyBegin = true;
                        Clear();
                        ChangeEvent(currentSpeed, currentHeight);
                        if (currentSpeed == 1000)
                        {
                            IsSpeedGained = true;
                            WriteLine(str = "\nВы набрали максимальную скорость. Ваша задача - посадить самолет!\a");
                            Record(str);
                        }
                        else if (IsSpeedGained && currentSpeed <= 50)
                        {
                            WriteLine("\nПолет закончился!\a");

                            WriteLine($"Сумарное число штрафных очков: {totalPenalty}\a");

                            break;
                        }

                        WriteLine(str = $"Скорость: {currentSpeed} км/ч Высота: {currentHeight} м");
                        Record(str);
                        WriteLine(str = $"{d1.Name} дал(а) суммарно {d1.Penalty} штрафных баллов");
                        Record(str);
                        totalPenalty = d1.Penalty;
                        if (totalPenalty >= 1000)
                            throw new Unsuitable("Непригоден к полетам");
                        distance += 50;
                        ChangeEvent -= d1.RecomendedHight;
                    }
                    else
                    {
                        totalPenalty = d1.Penalty + d2.Penalty;
                        if (totalPenalty >= 1000)
                            throw new Unsuitable("Непригоден к полетам");// Генерирует исключение «Непригоден к полетам»
                        ChangeEvent += d2.RecomendedHight;
                        Clear();

                        ChangeEvent(currentSpeed, currentHeight);
                        if (currentSpeed == 1000)
                        {
                            IsSpeedGained = true;
                            WriteLine(str = "\nВы набрали максимальную скорость. Ваша задача - посадить самолет!\a");
                            Record(str);
                        }
                        else if (IsSpeedGained && currentSpeed <= 50)// Управление самолетом диспетчерами прекращается
                        {
                            System.Media.SoundPlayer player = new System.Media.SoundPlayer(@"bugle.wav");
                            player.Play();
                            WriteLine("\nПолет закончился успешно!\n");
                            WriteLine($"Сумарное число штрафных очков: {totalPenalty}\n");
                            WriteLine($"{d1.Name} дал(а) суммарно {d1.Penalty} штрафных баллов");
                            WriteLine($"{d2.Name} дал(а) суммарно {d2.Penalty} штрафных баллов");

                            break;

                        }

                        WriteLine(str = $"Скорость: {currentSpeed} км/ч Высота: {currentHeight} м");
                        Record(str);
                        WriteLine(str = $"{d2.Name} дал(а) суммарно {d2.Penalty} штрафных баллов");
                        Record(str);
                        WriteLine(str = $"\n\nСумарное число штрафных очков по двум диспетчерам: {totalPenalty}\n");
                        Record(str);
                        ChangeEvent -= d2.RecomendedHight;

                    }



                }


            }



        }
    }
}