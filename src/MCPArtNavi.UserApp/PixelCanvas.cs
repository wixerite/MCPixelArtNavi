using System;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

using MCPArtNavi.Common;
using MCPArtNavi.UserApp.PixelCanvasInternal;

namespace MCPArtNavi.UserApp
{
    public class PixelCanvas : UserControl
    {
        // 非公開フィールド

        //private FastCanvas _mainCanvas;
        private Canvas _mainCanvas;
        private Rectangle[][] _pixRectangels;
        private Rectangle[] _chunksVerticalLines;
        private Rectangle[] _chunksHorizontalLines;
        private SolidColorBrush _defaultColorBrush;
        private SolidColorBrush _defaultChunkLineColorBrush;


        // 依存関係プロパティ

        public int PixelWidth
        {
            get { return (int)GetValue(PixelWidthProperty); }
            set { SetValue(PixelWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PixelWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PixelWidthProperty =
            DependencyProperty.Register("PixelWidth", typeof(int), typeof(PixelCanvas), new PropertyMetadata(0, new PropertyChangedCallback((sender, e) => ((PixelCanvas)sender)._tryCanvasUpdating())));

        public int PixelHeight
        {
            get { return (int)GetValue(PixelHeightProperty); }
            set { SetValue(PixelHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PixelHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PixelHeightProperty =
            DependencyProperty.Register("PixelHeight", typeof(int), typeof(PixelCanvas), new PropertyMetadata(0, new PropertyChangedCallback((sender, e) => ((PixelCanvas)sender)._tryCanvasUpdating())));

        public PixelCanvasUpdateMode UpdateMode
        {
            get { return (PixelCanvasUpdateMode)GetValue(UpdateModeProperty); }
            set { SetValue(UpdateModeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for UpdateMode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UpdateModeProperty =
            DependencyProperty.Register("UpdateMode", typeof(PixelCanvasUpdateMode), typeof(PixelCanvas), new PropertyMetadata(PixelCanvasUpdateMode.Freezed, new PropertyChangedCallback((sender, e) => ((PixelCanvas)sender)._tryCanvasUpdating())));




        public PixelCanvasMapHandler CanvasMapHandler
        {
            get { return (PixelCanvasMapHandler)GetValue(CanvasMapHandlerProperty); }
            set { SetValue(CanvasMapHandlerProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CanvasMapHandler.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CanvasMapHandlerProperty =
            DependencyProperty.Register("CanvasMapHandler", typeof(PixelCanvasMapHandler), typeof(PixelCanvas), new PropertyMetadata(null, new PropertyChangedCallback((sender, e) =>
            {
                var _this = (PixelCanvas)sender;
                if (e.OldValue as PixelCanvasMapHandler != null)
                {
                    var oldHandler = (PixelCanvasMapHandler)e.OldValue;
                    oldHandler.GetPixelRequested -= _this._mapHandler_getPixelRequested;
                    oldHandler.SetPixelRequested -= _this._mapHandler_setPixelRequested;
                }

                if (e.NewValue as PixelCanvasMapHandler != null)
                {
                    var newHandler = (PixelCanvasMapHandler)e.NewValue;
                    newHandler.GetPixelRequested += _this._mapHandler_getPixelRequested;
                    newHandler.SetPixelRequested += _this._mapHandler_setPixelRequested;
                }
            })));


        // コンストラクタ

        public PixelCanvas()
        {
            this._mainCanvas = new Canvas();
            this.Content = this._mainCanvas;

            this._defaultColorBrush = new SolidColorBrush(Colors.Silver);
            this._defaultChunkLineColorBrush = new SolidColorBrush(Colors.Blue);
            this._initalizeRectangles();
        }



        // 限定公開メソッド

        protected override void OnRender(DrawingContext drawingContext)
        {
            // 基底呼び出し
            base.OnRender(drawingContext);

            var rect = new Rect();

            // オブジェクト描画 :: ピクセル
            for (var i = 0; i < this.PixelWidth; i++)
            {
                for (var j = 0; j < this.PixelHeight; j++)
                {
                    var rectangle = this._pixRectangels[i][j];
                    rect.X = (double)rectangle.GetValue(Canvas.LeftProperty);
                    rect.Y = (double)rectangle.GetValue(Canvas.TopProperty);
                    rect.Width = rectangle.Width;
                    rect.Height = rectangle.Height;

                    drawingContext.DrawRectangle(this._pixRectangels[i][j].Fill, null, rect);
                }
            }

            // オブジェクト描画 :: チャンク線
            for (var i = 0; i < this._chunksVerticalLines.Length; i++)
            {
                var rectangle = this._chunksVerticalLines[i];
                rect.X = (double)rectangle.GetValue(Canvas.LeftProperty);
                rect.Y = (double)rectangle.GetValue(Canvas.TopProperty);
                rect.Width = rectangle.Width;
                rect.Height = rectangle.Height;

                drawingContext.DrawRectangle(this._chunksVerticalLines[i].Fill, null, rect);
            }

            for (var i = 0; i < this._chunksHorizontalLines.Length; i++)
            {
                var rectangle = this._chunksHorizontalLines[i];
                rect.X = (double)rectangle.GetValue(Canvas.LeftProperty);
                rect.Y = (double)rectangle.GetValue(Canvas.TopProperty);
                rect.Width = rectangle.Width;
                rect.Height = rectangle.Height;

                drawingContext.DrawRectangle(this._chunksHorizontalLines[i].Fill, null, rect);
            }
        }


        // 非公開メソッド

        private void _tryCanvasUpdating()
        {
            if (this.UpdateMode == PixelCanvasUpdateMode.Freezed)
                return;

            this._initalizeRectangles();
            this._initializeChunkLines();
            GC.Collect();
        }

        private void _removeElementsFromCanvas(IEnumerable<UIElement> elements)
        {
            foreach (var elem in elements)
                this._mainCanvas.Children.Remove(elem);
        }

        private void _initalizeRectangles()
        {
            if (this._pixRectangels != null)
            {
                for (var i = 0; i < this._pixRectangels.GetLength(0); i++)
                    this._removeElementsFromCanvas(this._pixRectangels[i]);
            }

            this._pixRectangels = new Rectangle[this.PixelHeight][];

            for (var i = 0; i < this.PixelHeight; i++)
            {
                this._pixRectangels[i] = new Rectangle[this.PixelWidth];
                //var loc = new Point();
                for (var j = 0; j < this.PixelWidth; j++)
                {
                    //loc.X = j + 0.2;
                    //loc.Y = i + 0.2;

                    this._pixRectangels[i][j] = new Rectangle();
                    this._pixRectangels[i][j].SetValue(Canvas.TopProperty, i + 0.2); // 行
                    this._pixRectangels[i][j].SetValue(Canvas.LeftProperty, j + 0.2); // 列
                    //this._pixRectangels[i][j].SetValue(FastCanvas.LocationProperty, loc);
                    this._pixRectangels[i][j].Width = 0.8d;
                    this._pixRectangels[i][j].Height = 0.8d;
                    this._pixRectangels[i][j].Fill = this._defaultColorBrush;

                    //this._mainCanvas.Children.Add(this._pixRectangels[i][j]);
                }
            }
        }

        private void _initializeChunkLines()
        {
            if (this._chunksVerticalLines != null)
                this._removeElementsFromCanvas(this._chunksVerticalLines);

            if (this._chunksHorizontalLines != null)
                this._removeElementsFromCanvas(this._chunksHorizontalLines);

            var chunkSize = 16;
            //var loc = new Point();

            // Vertical lines
            this._chunksVerticalLines = new Rectangle[(this.PixelWidth / chunkSize) + 1];
            for (var i = 0; i < this._chunksVerticalLines.Length; i++)
            {
                //loc.X = i * (double)chunkSize;
                //loc.Y = 0d;

                this._chunksVerticalLines[i] = new Rectangle();
                this._chunksVerticalLines[i].SetValue(Canvas.TopProperty, 0d);
                this._chunksVerticalLines[i].SetValue(Canvas.LeftProperty, i * (double)chunkSize);
                //this._chunksVerticalLines[i].SetValue(FastCanvas.LocationProperty, loc);
                this._chunksVerticalLines[i].Width = 0.2d;
                this._chunksVerticalLines[i].Height = (double)this.PixelHeight;
                this._chunksVerticalLines[i].Fill = this._defaultChunkLineColorBrush;

                //this._mainCanvas.Children.Add(this._chunksVerticalLines[i]);
            }

            // Horizontal lines
            this._chunksHorizontalLines = new Rectangle[(this.PixelHeight / chunkSize) + 1];
            for (var i = 0; i < this._chunksHorizontalLines.Length; i++)
            {
                //loc.X = 0d;
                //loc.Y = i * (double)chunkSize;

                this._chunksHorizontalLines[i] = new Rectangle();
                this._chunksHorizontalLines[i].SetValue(Canvas.TopProperty, i * (double)chunkSize);
                this._chunksHorizontalLines[i].SetValue(Canvas.LeftProperty, 0d);
                //this._chunksHorizontalLines[i].SetValue(FastCanvas.LocationProperty, loc);
                this._chunksHorizontalLines[i].Width = (double)this.PixelWidth;
                this._chunksHorizontalLines[i].Height = 0.2d;
                this._chunksHorizontalLines[i].Fill = this._defaultChunkLineColorBrush;

                //this._mainCanvas.Children.Add(this._chunksHorizontalLines[i]);
            }
        }

        private void _mapHandler_getPixelRequested(Object sender, PixelCanvasMapHandler.PixelEventArgs e)
        {
            if (this.PixelWidth < e.X || this.PixelHeight < e.Y)
                throw new ArgumentOutOfRangeException();

            var pixBrush = this._pixRectangels[e.Y][e.X].Fill;
            if (pixBrush == this._defaultColorBrush)
                e.Brush = null;
            else
                e.Brush = pixBrush;
        }

        private void _mapHandler_setPixelRequested(Object sender, PixelCanvasMapHandler.PixelEventArgs e)
        {
            if (this.PixelWidth < e.X || this.PixelHeight < e.Y)
                throw new ArgumentOutOfRangeException();

            if (e.Brush == null)
                this._pixRectangels[e.Y][e.X].Fill = this._defaultColorBrush;
            else
                this._pixRectangels[e.Y][e.X].Fill = e.Brush;
        }
    }
}
