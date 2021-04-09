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
    public class VerticalRuler : UserControl
    {
        // 非公開フィールド
        private Brush _rulerBrush;

        // コンストラクタ
        public VerticalRuler()
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

            for (var i = 0; i < this.Height + 1; i += 1)
            {
                var rect = new Rect(10, i, 10, 0.05);
                if (i % 4 == 0)
                {
                    rect.X = 0;
                    rect.Width = 20;
                }

                drawingContext.DrawRectangle(this._rulerBrush, null, rect);
            }
        }
    }
}
