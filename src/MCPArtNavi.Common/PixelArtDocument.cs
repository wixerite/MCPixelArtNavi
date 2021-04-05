using System;
using System.Collections.Generic;
using System.Text;

namespace MCPArtNavi.Common
{
    public class PixelArtDocument
    {
        // 非公開フィールド

        private IMCItem[] _pixels;


        // 公開プロパティ

        public PixelArtSize Size
        {
            get;
            set;
        }

        public IMCItem[] Pixels
        {
            get => this._pixels;
            set
            {
                this._validatePixelsLength(value);
                this._pixels = value;
            }
        }

        public string DocumentTitle
        {
            get;
            set;
        }


        // 非公開メソッド

        private void _validatePixelsLength(IMCItem[] pixels)
        {
            var w = this.Size.GetWidth();
            var h = this.Size.GetHeight();

            if (w * h != pixels.Length)
                throw new ArgumentOutOfRangeException();
        }
    }
}
