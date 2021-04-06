using System;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

using MCPArtNavi.Common;
using MCPArtNavi.UserApp.PixelCanvasInternal;

namespace MCPArtNavi.UserApp
{
    public class PixelCanvas : UserControl
    {
        // 非公開フィールド

        //private FastCanvas _mainCanvas;
        //private Canvas _mainCanvas;
        //private Canvas _dummyCanvas;
        private RenderRectangle[][] _pixRectangels;
        private RenderRectangle[] _chunksVerticalLines;
        private RenderRectangle[] _chunksHorizontalLines;
        private SolidColorBrush _defaultColorBrush;
        private SolidColorBrush _defaultChunkLineColorBrush;

        private DrawingGroup _backingStore;


        // 依存関係プロパティ

        public int PixelWidth
        {
            get { return (int)GetValue(PixelWidthProperty); }
            set { SetValue(PixelWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PixelWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PixelWidthProperty =
            DependencyProperty.Register("PixelWidth", typeof(int), typeof(PixelCanvas), new PropertyMetadata(0, new PropertyChangedCallback((sender, e) => ((PixelCanvas)sender)._canvasLayoutUpdating())));

        public int PixelHeight
        {
            get { return (int)GetValue(PixelHeightProperty); }
            set { SetValue(PixelHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PixelHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PixelHeightProperty =
            DependencyProperty.Register("PixelHeight", typeof(int), typeof(PixelCanvas), new PropertyMetadata(0, new PropertyChangedCallback((sender, e) => ((PixelCanvas)sender)._canvasLayoutUpdating())));


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
                    oldHandler.RedrawPixelsRequested -= _this._mapHandler_redrawPixelsRequested;
                }

                if (e.NewValue as PixelCanvasMapHandler != null)
                {
                    var newHandler = (PixelCanvasMapHandler)e.NewValue;
                    newHandler.GetPixelRequested += _this._mapHandler_getPixelRequested;
                    newHandler.SetPixelRequested += _this._mapHandler_setPixelRequested;
                    newHandler.RedrawPixelsRequested += _this._mapHandler_redrawPixelsRequested;
                }
            })));


        // コンストラクタ

        public PixelCanvas()
        {
            this._backingStore = new DrawingGroup();

            this._defaultColorBrush = new SolidColorBrush(Colors.Silver);
            this._defaultChunkLineColorBrush = new SolidColorBrush(Colors.Blue);
            this._canvasLayoutUpdating();
        }



        // 限定公開メソッド

        protected override void OnRender(DrawingContext drawingContext)
        {
            // 基底呼び出し
            base.OnRender(drawingContext);

            // 描画

            // オブジェクト描画 :: ピクセル
            for (var i = 0; i < this.PixelWidth; i++)
            {
                for (var j = 0; j < this.PixelHeight; j++)
                {
                    var rr = this._pixRectangels[i][j];
                    drawingContext.DrawRectangle(rr.Brush, null, rr.Rect);
                }
            }

            // オブジェクト描画 :: チャンク線
            for (var i = 0; i < this._chunksVerticalLines.Length; i++)
            {
                var rr = this._chunksVerticalLines[i];
                drawingContext.DrawRectangle(rr.Brush, null, rr.Rect);
            }

            for (var i = 0; i < this._chunksHorizontalLines.Length; i++)
            {
                var rr = this._chunksHorizontalLines[i];
                drawingContext.DrawRectangle(rr.Brush, null, rr.Rect);
            }
        }


        // 非公開メソッド

        /// <summary>
        /// 縦横サイズやチャンク線の位置を現在のサイズ パラメータに合わせて初期化します。
        /// </summary>
        private void _canvasLayoutUpdating()
        {
            this._initalizeRectangles();
            this._initializeChunkLines();
        }

        /// <summary>
        /// 描画を行います。
        /// </summary>
        private void _redrawPixels()
        {
            this.InvalidateVisual();
        }

        private void _initalizeRectangles()
        {
            this._pixRectangels = new RenderRectangle[this.PixelHeight][];

            for (var i = 0; i < this.PixelHeight; i++)
            {
                this._pixRectangels[i] = new RenderRectangle[this.PixelWidth];
                for (var j = 0; j < this.PixelWidth; j++)
                {
                    this._pixRectangels[i][j] = new RenderRectangle();
                    this._pixRectangels[i][j].Brush = this._defaultColorBrush;
                    this._pixRectangels[i][j].Rect = new Rect(j + 0.2, i + 0.2, 0.8d, 0.8d);
                }
            }
        }

        private void _initializeChunkLines()
        {
            var chunkSize = 16;

            // Vertical lines
            this._chunksVerticalLines = new RenderRectangle[(this.PixelWidth / chunkSize) + 1];
            for (var i = 0; i < this._chunksVerticalLines.Length; i++)
            {
                this._chunksVerticalLines[i] = new RenderRectangle();
                this._chunksVerticalLines[i].Brush = this._defaultChunkLineColorBrush;
                this._chunksVerticalLines[i].Rect = new Rect(i * (double) chunkSize, 0d, 0.2d, (double)this.PixelHeight);
            }

            // Horizontal lines
            this._chunksHorizontalLines = new RenderRectangle[(this.PixelHeight / chunkSize) + 1];
            for (var i = 0; i < this._chunksHorizontalLines.Length; i++)
            {
                this._chunksHorizontalLines[i] = new RenderRectangle();
                this._chunksHorizontalLines[i].Brush = this._defaultChunkLineColorBrush;
                this._chunksHorizontalLines[i].Rect = new Rect(0d, i * (double)chunkSize, (double)this.PixelWidth, 0.2d);
            }
        }

        private void _mapHandler_getPixelRequested(Object sender, PixelCanvasMapHandler.PixelEventArgs e)
        {
            if (this.PixelWidth < e.X || this.PixelHeight < e.Y)
                throw new ArgumentOutOfRangeException();

            var pixBrush = this._pixRectangels[e.Y][e.X].Brush;
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
                this._pixRectangels[e.Y][e.X].Brush = this._defaultColorBrush;
            else
                this._pixRectangels[e.Y][e.X].Brush = e.Brush;
        }

        private void _mapHandler_redrawPixelsRequested(Object sender, EventArgs e)
        {
            this._redrawPixels();
        }
    }
}
