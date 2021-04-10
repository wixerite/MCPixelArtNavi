using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MCPArtNavi.Common;

namespace MCPArtNavi.Importer
{
    public abstract class ImporterBase
    {
        // 公開プロパティ

        public abstract PixelArtSize TargetSize
        {
            get;
        }

        public abstract ImporterPalette ItemPalette
        {
            get;
        }


        // 公開メソッド

        public abstract Task<PixelArtDocument> ImportAsync(Stream stream, string documentTitle);
    }
}
