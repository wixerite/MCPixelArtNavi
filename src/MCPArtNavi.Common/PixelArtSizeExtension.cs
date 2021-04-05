using System;
using System.Collections.Generic;
using System.Text;

namespace MCPArtNavi.Common
{
    public static class PixelArtSizeExtension
    {
        private static string _getSizeStr(PixelArtSize size)
        {
            return size.ToString().Replace("Size", "\n").Split('\n')[1];
        }


        public static int GetWidth(this PixelArtSize size)
        {
            return Int32.Parse(_getSizeStr(size).Split('x')[0]);
        }

        public static int GetHeight(this PixelArtSize size)
        {
            return Int32.Parse(_getSizeStr(size).Split('x')[1]);
        }
    }
}
