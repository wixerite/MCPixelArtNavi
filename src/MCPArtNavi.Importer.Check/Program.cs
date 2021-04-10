using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCPArtNavi.Importer.Check
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Importer-lib-check");
            Console.WriteLine();

            Check("XyzDiff", ForCheck.GetXyzDiff);

            //var ii = new ImageImporter();
            //Console.WriteLine("Nearly White: {0}", ii.);

            Console.WriteLine();
            Console.ReadKey();
        }

        static void Check(string algoName, Func<Color, Color, double> diffAction)
        {
            Console.WriteLine("[Algo: {0}]", algoName);

            Console.WriteLine("Black vs White: {0}", diffAction(Color.Black, Color.White));
            Console.WriteLine("Black vs Silver: {0}", diffAction(Color.Black, Color.Silver));
            Console.WriteLine("Black vs Gray: {0}", diffAction(Color.Black, Color.Gray));
            Console.WriteLine("Black vs Black: {0}", diffAction(Color.Black, Color.Black));

            Console.WriteLine("Red vs Red: {0}", diffAction(Color.Red, Color.Red));
            Console.WriteLine("Red vs Green: {0}", diffAction(Color.Red, Color.Green));
            Console.WriteLine("Red vs Blue: {0}", diffAction(Color.Red, Color.Blue));

            Console.WriteLine();
        }
    }
}
