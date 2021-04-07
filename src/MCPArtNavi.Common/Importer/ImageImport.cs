using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MCPArtNavi.Common.Items;

namespace MCPArtNavi.Common.Importer
{
    public class ImageImport
    {
        public PixelArtSize TargetSize
        {
            get;
            private set;
        }

        public IMCItem[] Palette
        {
            get;
            private set;
        }


        // コンストラクタ

        public ImageImport(PixelArtSize targetSize, IMCItem[] palette)
        {
            this.TargetSize = targetSize;
            this.Palette = palette;
        }

        public ImageImport(PixelArtSize targetSize)
            : this(targetSize, MCItemUtils.EnabledItems.ToArray())
        {
            // NOP
        }


        // 公開メソッド

    }
}
