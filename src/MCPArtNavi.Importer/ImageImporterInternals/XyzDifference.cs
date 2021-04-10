using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCPArtNavi.Importer.ImageImporterInternals
{
    internal static class XyzDifference
    {
        // 非公開静的フィールド
        private static double[][] _m;
        private static double[][] _mi;
        private static double _max;


        // 静的コンストラクタ

        /// <summary>
        /// <see cref="XyzDifference"/> クラスを初期化します。
        /// </summary>
        static XyzDifference()
        {
            _m = new double[][]
            {
                new double[] {0.4124d, 0.3576d, 0.1805d},
                new double[] {0.2126d, 0.7152d, 0.0722d},
                new double[] {0.0193d, 0.1192d, 0.9505d}
            };

            double m11 = _m[0][0], m12 = _m[0][1], m13 = _m[0][2];
            double m21 = _m[1][0], m22 = _m[1][1], m23 = _m[1][2];
            double m31 = _m[2][0], m32 = _m[2][1], m33 = _m[2][2];

            var d = m11 * m22 * m33
                + m21 * m32 * m13
                + m31 * m12 * m23
                - m11 * m32 * m23
                - m31 * m22 * m13
                - m21 * m12 * m33;

            _mi = new double[][]{
                new double[] { (m22*m33-m23*m32)/d,(m13*m32-m12*m33)/d,(m12*m23-m13*m22)/d },
                new double[] { (m23*m31-m21*m33)/d,(m11*m33-m13*m31)/d,(m13*m21-m11*m23)/d },
                new double[] { (m21*m32-m22*m31)/d,(m12*m31-m11*m32)/d,(m11*m22-m12*m21)/d }
            };

            _max = _diff(Color.White, Color.Black);
        }


        // 非公開静的メソッド

        private static double _getX(double r, double g, double b)
        {
            return _m[0][0] * r + _m[0][1] * g + _m[0][2] * b;
        }
        private static double _getY(double r, double g, double b)
        {
            return _m[1][0] * r + _m[1][1] * g + _m[1][2] * b;
        }
        private static double _getZ(double r, double g, double b)
        {
            return _m[2][0] * r + _m[2][1] * g + _m[2][2] * b;
        }
        private static double _getR(double[] xyz)
        {
            return _mi[0][0] * xyz[0] + _mi[0][1] * xyz[1] + _mi[0][2] * xyz[2];
        }
        private static double _getG(double[] xyz)
        {
            return _mi[1][0] * xyz[0] + _mi[1][1] * xyz[1] + _mi[1][2] * xyz[2];
        }
        private static double _getB(double[] xyz)
        {
            return _mi[2][0] * xyz[0] + _mi[2][1] * xyz[1] + _mi[2][2] * xyz[2];
        }

        private static double[] _rgbToXyz(Color c)
        {
            return new double[]{
                _getX(c.R / 255d, c.G / 255d, c.B / 255d),
                _getY(c.R / 255d, c.G / 255d, c.B / 255d),
                _getZ(c.R / 255d, c.G / 255d, c.B / 255d)
            };
        }

        private static Color xyz2rgb(double[] xyz)
        {
            return Color.FromArgb(
                (int)(_getR(xyz) * 255),
                (int)(_getG(xyz) * 255),
                (int)(_getB(xyz) * 255));
        }

        private static double _diff(Color src, Color dst)
        {
            var s = _rgbToXyz(src);
            var d = _rgbToXyz(dst);

            double xd = s[0] - d[0];
            double yd = s[1] - d[1];
            double zd = s[2] - d[2];

            return Math.Sqrt(xd * xd + yd * yd + zd * zd);
        }


        // 公開静的メソッド

        public static double Difference(Color src, Color dst)
        {
            return _diff(src, dst) / _max;
        }
    }
}
