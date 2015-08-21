using System;
using System.IO;

namespace AtelierXNA
{
    static class Program
    {
        static void Main(string[] args)
        {
            using (RPG game = new RPG())
            {
                //try
                //{
                //    game.Run();
                //}
                //catch (Exception e)
                //{
                //    Debug.WriteLine(e.Message);
                //    StreamWriter fichierSortie = new StreamWriter("debug_log.txt");
                //    fichierSortie.WriteLine("Message: " + e.Message);
                //    fichierSortie.WriteLine("At:      " + e.TargetSite);
                //    fichierSortie.WriteLine("Trace:   " + e.StackTrace);
                //    fichierSortie.Close();
                //}
                game.Run();
            }
        }
    }
}

