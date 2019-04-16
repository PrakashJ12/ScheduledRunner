using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Write500Lines
{
    class Program
    {
        public static void Main()
        {
            for (int ctr = 0; ctr < 500; ctr++)
            {
                Console.WriteLine($"Line {ctr + 1} of 500 written: {ctr + 1 / 500.0:P2}");
                System.Threading.Thread.Sleep(500);
            }
                
            //Console.Error.WriteLine("\nSuccessfully wrote 500 lines.\n");
        }
    }
}
