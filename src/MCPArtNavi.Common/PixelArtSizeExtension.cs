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

        public static string ToNameString(this PixelArtSize size)
        {
            var result = $"Width={size.GetWidth()}, Height={size.GetHeight()}";
            
            switch (size)
            {
                case PixelArtSize.Size256x128:
                    result += " (2:1)";
                    break;
                case PixelArtSize.Size256x144:
                    result += " (16:9)";
                    break;
            }

            return result;
        }
    }
}
