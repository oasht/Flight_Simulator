using System;
using System.IO;
using System.Text;
using static System.Console;
namespace Flight_Simulator
{
    class AirplaneCrushed : Exception
    {
        public AirplaneCrushed(string message)
            : base(message)
        { }
    }
    class Unsuitable : Exception
    {
        public Unsuitable(string message)
            : base(message)
        { }
    }

    class Program
    {
        public static void Open_Black_Box()
        {
            string fPath = "Record.txt";
            WriteLine("\n\nЕсли хотите открыть черный ящик нажмите 1");
            short key_box = short.Parse(Console.ReadLine());

            if (key_box == 1)
            {
                Clear();
                using (FileStream fs = new FileStream(fPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    byte[] str_byte = new byte[(int)fs.Length];
                    fs.Read(str_byte, 0, str_byte.Length);
                    string str_new = Encoding.Unicode.GetString(str_byte);
                    WriteLine(str_new);
                }
            }
            else
                Environment.Exit(0);
        }
        static void Main(string[] args)
        {

            try
            {
                Airplane plane = new Airplane();
                plane.Fly();
            }
            catch (AirplaneCrushed ac)
            {
                Console.WriteLine(ac.Message);
                System.Media.SoundPlayer player = new System.Media.SoundPlayer(@"regret.wav");
                player.Play();
                WriteLine(@"          
          _ ._  _ , _ ._
        (_ ' ( `  )_  .__)
      ( (  (    )   `)  ) _)
     (__ (_   (_ . _) _) ,__)
         `~~`\ ' . /`~~`
              ;   ;
              /   \
_____________/_ __ \_____________");
                Open_Black_Box();

            }
            catch (Unsuitable u)
            {
                Console.WriteLine(u.Message);
                Console.Beep();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
