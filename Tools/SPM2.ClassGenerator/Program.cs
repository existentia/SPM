using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SPM2.ClassGenerator
{
    class Program
    {
        static void Main(string[] args)
        {

            Genereator g = new Genereator();
            g.Run();


            Console.WriteLine("Done!");
#if SPSE
            // Console.ReadKey throws when stdin is redirected, so only wait when the
            // generator is actually being driven by a human at a console.
            if (!Console.IsInputRedirected)
            {
                Console.ReadKey();
            }
#else
            Console.ReadKey();
#endif

        }
    }
}
