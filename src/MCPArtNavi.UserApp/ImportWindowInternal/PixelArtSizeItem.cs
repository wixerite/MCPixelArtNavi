using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MCPArtNavi.Common;

namespace MCPArtNavi.UserApp.ImportWindowInternal
{
    public class PixelArtSizeItem
    {
        private string _text;


        public PixelArtSize Value
        {
            get;
            private set;
        }

        public override string ToString()
        {
            return this._text;
        }

        public PixelArtSizeItem(PixelArtSize value)
        {
            this.Value = value;
            this._text = this.Value.GetWidth() + "x" + this.Value.GetHeight();
        }
    }
}
