using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MCPArtNavi.UserApp
{
    public class PixelCanvasMapHandler
    {
        // 非公開フィールド
        private EventHandler<PixelEventArgs> _getPixelRequested;
        private EventHandler<PixelEventArgs> _setPixelRequested;
        private EventHandler _redrawPixelsRequested;


        // 公開イベント

        public event EventHandler<PixelEventArgs> GetPixelRequested
        {
            add => this._getPixelRequested += value;
            remove => this._getPixelRequested -= value;
        }

        public event EventHandler<PixelEventArgs> SetPixelRequested
        {
            add => this._setPixelRequested += value;
            remove => this._setPixelRequested -= value;
        }

        public event EventHandler RedrawLayoutRequested
        {
            add => this._redrawPixelsRequested += value;
            remove => this._redrawPixelsRequested -= value;
        }



        // 公開メソッド

        public Brush GetPixel(int x, int y)
        {
            try
            {
                var eventArgs = new PixelEventArgs() { X = x, Y = y };
                this._getPixelRequested?.Invoke(this, eventArgs);
                return eventArgs.Brush;
            }
            catch
            {
                // NOP
            }

            return null;
        }

        public bool SetPixel(int x, int y, Brush brush)
        {
            try
            {
                this._setPixelRequested?.Invoke(this, new PixelEventArgs()
                {
                    X = x,
                    Y = y,
                    Brush = brush
                });
            }
            catch
            {
                return false;
            }

            return true;
        }

        public void RedrawLayout()
        {
            this._redrawPixelsRequested?.Invoke(this, new EventArgs());
        }

        
        // その他

        public class PixelEventArgs
        {
            public int X { get; set; }

            public int Y { get; set; }

            public Brush Brush { get; set; }
        }
    }
}
