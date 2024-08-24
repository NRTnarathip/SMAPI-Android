using System;

namespace LearnCode
{
    internal class Program
    {
        enum Ghost
        {
            None, Noob, Good, Smart
        }
        static void Main(string[] args)
        {
            var enumName = Ghost.None.ToString();
            Console.WriteLine();
        }
    }
}
