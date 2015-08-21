using System;
using System.Runtime.InteropServices;

namespace AtelierXNA
{
    public static class Debug
    {
        [DllImport("kernel32")]
        static extern bool AllocConsole();

        static Debug()
        {
            AllocConsole();
        }

        public static void WriteLine(string text)
        {
            Console.WriteLine(text);
        }

        public static void Write(string text)
        {
            Console.Write(text);
        }

        public static void Clear()
        {
            Console.Clear();
        }

        public static string ReadLine()
        {
            return Console.ReadLine();
        }
    }
}
