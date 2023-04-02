using System;
using System.Data;
using System.IO;
using System.Text;
using System.Threading;
using static System.Console;

namespace Flight_Simulator
{

    class Dispatcher
    {
        public string Name { get; set; }
        public int Penalty { get; set; }
        private int adjustment;
        private static Random r;

        public Dispatcher(string name)
        {
            Name = name;
            r = new Random();
            adjustment = r.Next(-200, 200);
            Penalty = 0;
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

        public void RecomendedHight(int speed, int height)
        {
            string str = "";
            int recomended = 7 * speed - adjustment;//Рекомендованная высота

            int difference;// Разница между рекомендованой и текущей высотой
            if (height > recomended)
                difference = height - recomended;
            else
                difference = recomended - height;
            Clear();
            WriteLine(str = $"Диспетчер {Name}: Рекомендуемая высота полета: {recomended} м.");
            Record(str);


            if (speed > 1000)// Превышение максимальной скорости
            {
                Clear();
                Penalty += 100;
                WriteLine(str = $"Диспетчер {Name}: Немедленно снизьте скорость!");
                Record(str);
                WriteLine(str = $"Диспетчер {Name}: 100 баллов штраф!");
                Record(str);


            }

            if (difference >= 300 && difference < 600)//Штраф_1
            {
                System.Media.SoundPlayer player = new System.Media.SoundPlayer(@"button.wav");
                player.Play();
                Penalty += 25;
                WriteLine(str = $"Диспетчер {Name}: 25 баллов штраф!");
                Record(str);


            }
            else if (difference >= 600 && difference < 1000)//Штраф_2
            {
                System.Media.SoundPlayer player = new System.Media.SoundPlayer(@"button.wav");
                player.Play();

                Penalty += 50;
                WriteLine(str = $"Диспетчер {Name}: 50 баллов штраф!");
                Record(str);

            }
            else if (difference >= 1000 || (speed <= 0 && height <= 0))
                throw new AirplaneCrushed("Самолет разбился");

            if (Penalty >= 1000)
                throw new Unsuitable("Непригоден к полетам");
        }
    }
}