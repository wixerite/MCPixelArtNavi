using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MCPArtNavi.UserApp.PixelCanvasInternal
{
    public class HandleableElement : UserControl
    {
        // 非公開フィールド
        private EventHandler<HandleableElementRenderingEventArgs> _rendering;


        // 公開イベント

        public event EventHandler<HandleableElementRenderingEventArgs> Rendering
        {
            add => this._rendering += value;
            remove => this._rendering -= value;
        }


        // 限定公開メソッド

        protected override void OnRender(DrawingContext drawingContext)
        {
            // 基底呼び出し
            base.OnRender(drawingContext);

            // イベント着火
            this._rendering?.Invoke(this, new HandleableElementRenderingEventArgs()
            {
                DrawingContext = drawingContext
            });
        }


        // その他

        public class HandleableElementRenderingEventArgs
        {
            public DrawingContext DrawingContext
            {
                get;
                set;
            }
        }
    }
}
