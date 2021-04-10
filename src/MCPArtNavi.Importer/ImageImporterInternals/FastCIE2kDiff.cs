using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCPArtNavi.Importer.ImageImporterInternals
{
    /// <summary>
    /// CIEDE2000 Logics
    /// Thanks for shinido. See https://qiita.com/shinido/items/2904fa1e9a6c78650b93
    /// </summary>
    internal static class FastCIE2kDiff
    {
        private static readonly double pi_2 = Math.PI * 2;
        private static readonly double v25_7 = Math.Pow(25, 7);
        private static readonly double d6 = MathEx.ToRadians(6);
        private static readonly double d25 = MathEx.ToRadians(25);
        private static readonly double d30 = MathEx.ToRadians(30);
        private static readonly double d60 = MathEx.ToRadians(60);
        private static readonly double d63 = MathEx.ToRadians(63);
        private static readonly double d275 = MathEx.ToRadians(275);
        private static readonly double kl = 1;
        private static readonly double kc = 1;
        private static readonly double kh = 1;
        private static readonly double MAX;

        static FastCIE2kDiff()
        {
            //MAX = diff(Color.White, Color.Black);
        }

        private static double _hypot(double a, double b)
        {
            return Math.Sqrt(a * a + b * b);
        }

        private static double _pow2(double a)
        {
            return a * a;
        }

        private static double _pow7(double a)
        {
            return a * a * a * a * a * a * a;
        }

        public static double Diff(Color src, Color dst)
        {
            var src_lab = Lab
        }
    }
}
