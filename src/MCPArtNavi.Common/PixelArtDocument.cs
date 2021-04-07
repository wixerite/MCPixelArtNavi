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


        // 公開静的メソッド

        public static PixelArtDocument GetEmptyDocument(PixelArtSize size, IMCItem fill)
        {
            var pixels = new IMCItem[size.GetWidth() * size.GetHeight()];
            for (var i = 0; i < pixels.Length; i++)
                pixels[i] = fill;

            var doc = new PixelArtDocument();
            doc.Size = PixelArtSize.Size128x128;
            doc.Pixels = pixels;

            return doc;
        }
    }
}
