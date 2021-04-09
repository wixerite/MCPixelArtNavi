using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MCPArtNavi.UserApp.PixelCanvasRulers
{
    public class HorizontalRuler : UserControl
    {
        // 非公開フィールド
        private Brush _rulerBrush;


        // コンストラクタ

        public HorizontalRuler()
        {
            this._rulerBrush = new SolidColorBrush(Colors.Black);
            this._rulerBrush.Freeze();
            
            this.SizeChanged += _sizeChanged;
        }

        private void _sizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.InvalidateVisual();
        }


        protected override void OnRender(DrawingContext drawingContext)
        {
            // 基底呼び出し
            base.OnRender(drawingContext);

            for (var i = 0; i < this.Width + 1; i += 1)
            {
                var rect = new Rect(i, 0, 0.05, 10);
                if (i % 4 == 0)
                    rect.Height = 20;

                drawingContext.DrawRectangle(this._rulerBrush, null, rect);
            }
        }
    }
}
