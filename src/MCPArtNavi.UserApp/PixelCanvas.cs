using System;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

using MCPArtNavi.Common;
using MCPArtNavi.UserApp.PixelCanvasInternal;

namespace MCPArtNavi.UserApp
{
    public class PixelCanvas : UserControl
    {
        // 非公開フィールド

        /// <summary>
        /// <see cref="PixelWidth"/> と同じ値が格納されます。<see cref="PixelWidth"/> は依存関係プロパティのため、別スレッドから利用ができないため、
        /// 値を取得する際は代わりにこちらを使用します。
        /// </summary>
        private int _pixelWidth;

        /// <summary>
        /// <see cref="PixelHeight"/> と同じ値が格納されます。<see cref="PixelHeight"/> は依存関係プロパティのため、別スレッドから利用ができないため、
        /// 値を取得する際は代わりにこちらを使用します。
        /// </summary>
        private int _pixelHeight;

        private Grid _rootLayer;
        private HandleableElement _pixelMapLayer;
        private HandleableElement _chunkLinesLayer;

        private RenderRectangle[][] _pixRectangels;
        private RenderRectangle[] _chunksVerticalLines;
        private RenderRectangle[] _chunksHorizontalLines;
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
                    oldHandler.RedrawLayoutRequested -= _this._mapHandler_redrawPixelsRequested;
                    oldHandler.CanvasToBitmapRequested -= _this._mapHandler_canvasToBitmapRequested;
                    oldHandler.InvokeDispatcherRequested -= _this._mapHandler_invokeDispatcherRequested;
                    
                }

                if (e.NewValue as PixelCanvasMapHandler != null)
                {
                    var newHandler = (PixelCanvasMapHandler)e.NewValue;
                    newHandler.GetPixelRequested += _this._mapHandler_getPixelRequested;
                    newHandler.SetPixelRequested += _this._mapHandler_setPixelRequested;
                    newHandler.RedrawLayoutRequested += _this._mapHandler_redrawPixelsRequested;
                    newHandler.CanvasToBitmapRequested += _this._mapHandler_canvasToBitmapRequested;
                    newHandler.InvokeDispatcherRequested += _this._mapHandler_invokeDispatcherRequested;
                }
            })));




        public Visibility ChunkLinesLayerVisibility
        {
            get { return (Visibility)GetValue(ChunkLinesLayerVisibilityProperty); }
            set { SetValue(ChunkLinesLayerVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ChunkLinesLayerVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ChunkLinesLayerVisibilityProperty =
            DependencyProperty.Register("ChunkLinesLayerVisibility", typeof(Visibility), typeof(PixelCanvas), new PropertyMetadata(Visibility.Visible, new PropertyChangedCallback((sender, e) =>
            {
                var _this = (PixelCanvas)sender;
                _this._chunkLinesLayer.Visibility = (Visibility)e.NewValue;
            })));





        // コンストラクタ

        public PixelCanvas()
        {
            this._rootLayer = new Grid();
            this.Content = this._rootLayer;

            this._chunkLinesLayer = new HandleableElement();
            this._chunkLinesLayer.Rendering += _chunkLinesLayer_Rendering;

            this._pixelMapLayer = new HandleableElement();
            this._pixelMapLayer.Rendering += _pixelMapLayer_Rendering;

            this._rootLayer.Children.Add(this._pixelMapLayer);
            this._rootLayer.Children.Add(this._chunkLinesLayer);

            this._defaultColorBrush = new SolidColorBrush(Colors.Silver);
            this._defaultChunkLineColorBrush = new SolidColorBrush(Colors.Blue);

            if (this._defaultColorBrush.CanFreeze)
                this._defaultColorBrush.Freeze();
            if (this._defaultChunkLineColorBrush.CanFreeze)
                this._defaultChunkLineColorBrush.Freeze();

            this._canvasLayoutUpdating();
        }


        // 非公開メソッド

        /// <summary>
        /// 縦横サイズやチャンク線の位置を現在のサイズ パラメータに合わせて初期化します。
        /// </summary>
        private void _canvasLayoutUpdating()
        {
            this.Dispatcher.Invoke(() =>
            {
                this._pixelWidth = this.PixelWidth;
                this._pixelHeight = this.PixelHeight;
            });

            this._initalizeRectangles();
            this._initializeChunkLines();
        }

        /// <summary>
        /// 描画を行います。
        /// </summary>
        private void _redrawLayout()
        {
            // Dispatcher 経由で改善なし
            System.Diagnostics.Debug.WriteLine("Redraw start (external)");
            this.Dispatcher.Invoke(() =>
            {
                try
                {
                    System.Diagnostics.Debug.WriteLine("Redraw start");
                    this._pixelMapLayer.InvalidateVisual();
                    this._chunkLinesLayer.InvalidateVisual();
                    System.Diagnostics.Debug.WriteLine("Redraw completed");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("_redrawLayout error!!");
                    throw ex;
                }
            }, DispatcherPriority.Send);
            System.Diagnostics.Debug.WriteLine("Redraw completed (external)");
        }

        private void _initalizeRectangles()
        {
            this._pixRectangels = new RenderRectangle[this._pixelHeight][];

            for (var i = 0; i < this._pixelHeight; i++)
            {
                this._pixRectangels[i] = new RenderRectangle[this._pixelWidth];
                for (var j = 0; j < this._pixelWidth; j++)
                {
                    this._pixRectangels[i][j] = new RenderRectangle();
                    this._pixRectangels[i][j].Brush = this._defaultColorBrush;
                    this._pixRectangels[i][j].Rect = new Rect(j + 0.07, i + 0.07, 0.93d, 0.93d);
                }
            }
        }

        private void _initializeChunkLines()
        {
            var chunkSize = 16;

            // Vertical lines
            this._chunksVerticalLines = new RenderRectangle[(this._pixelWidth / chunkSize) + 1];
            for (var i = 0; i < this._chunksVerticalLines.Length; i++)
            {
                this._chunksVerticalLines[i] = new RenderRectangle();
                this._chunksVerticalLines[i].Brush = this._defaultChunkLineColorBrush;
                this._chunksVerticalLines[i].Rect = new Rect(i * (double) chunkSize, 0d, 0.2d, (double)this._pixelHeight);
            }

            // Horizontal lines
            this._chunksHorizontalLines = new RenderRectangle[(this._pixelHeight / chunkSize) + 1];
            for (var i = 0; i < this._chunksHorizontalLines.Length; i++)
            {
                this._chunksHorizontalLines[i] = new RenderRectangle();
                this._chunksHorizontalLines[i].Brush = this._defaultChunkLineColorBrush;
                this._chunksHorizontalLines[i].Rect = new Rect(0d, i * (double)chunkSize, (double)this._pixelWidth, 0.2d);
            }
        }

        private void _mapHandler_getPixelRequested(Object sender, PixelCanvasMapHandler.PixelEventArgs e)
        {
            if (this._pixelWidth < e.X || this._pixelHeight < e.Y)
                throw new ArgumentOutOfRangeException();

            var pixBrush = this._pixRectangels[e.Y][e.X].Brush;
            if (pixBrush == this._defaultColorBrush)
                e.Brush = null;
            else
                e.Brush = pixBrush;
        }

        private void _mapHandler_setPixelRequested(Object sender, PixelCanvasMapHandler.PixelEventArgs e)
        {
            if (this._pixelWidth < e.X || this._pixelHeight < e.Y)
                throw new ArgumentOutOfRangeException();

            if (e.Brush == null)
                this._pixRectangels[e.Y][e.X].Brush = this._defaultColorBrush;
            else
                this._pixRectangels[e.Y][e.X].Brush = e.Brush;
        }

        private void _mapHandler_redrawPixelsRequested(Object sender, EventArgs e)
        {
            this._redrawLayout();
        }

        private void _mapHandler_canvasToBitmapRequested(Object sender, PixelCanvasMapHandler.CanvasToBitmapEventArgs e)
        {
            var bounds = VisualTreeHelper.GetDescendantBounds(this);
            bounds.Width *= 50;
            bounds.Height *= 50;

            var bitmap = new RenderTargetBitmap(
                (int)bounds.Width,
                (int)bounds.Height,
                96.0,
                96.0,
                PixelFormats.Pbgra32);

            var dv = new DrawingVisual();
            using (var dc = dv.RenderOpen())
            {
                var vb = new VisualBrush(this);
                dc.DrawRectangle(vb, null, bounds);
            }

            bitmap.Render(dv);
            bitmap.Freeze();

            e.Bitmap = bitmap;
        }

        private void _mapHandler_invokeDispatcherRequested(Object sender, PixelCanvasMapHandler.InvokeDispatcherEventArgs e)
        {
            this.Dispatcher.Invoke(e.Action);
        }


        // 非公開メソッド :: 子レイヤー描画

        private void _chunkLinesLayer_Rendering(object sender, HandleableElement.HandleableElementRenderingEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("_chunkLinesLayer_Rendering start");

            // Dispatcher 経由で改善なし
            var fe = (FrameworkElement)sender;

            System.Diagnostics.Debug.WriteLine("_chunkLinesLayer_Rendering vertical lines ...");
            for (var i = 0; i < this._chunksVerticalLines.Length; i++)
            {
                var rr = this._chunksVerticalLines[i];
                try
                {
                    if (i % 10 == 0)
                        System.Diagnostics.Debug.WriteLine("_chunkLinesLayer_Rendering pix exec");

                    fe.Dispatcher.Invoke(() =>
                    {
                        try
                        {
                            e.DrawingContext.DrawRectangle(rr.Brush, null, rr.Rect);
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine("_chunkLinesLayer_Rendering error!!");
                            throw ex;
                        }
                    });
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("_chunkLinesLayer_Rendering error!! (External)");
                    throw ex;
                }
            }

            System.Diagnostics.Debug.WriteLine("_chunkLinesLayer_Rendering horizontal lines ...");
            for (var i = 0; i < this._chunksHorizontalLines.Length; i++)
            {
                var rr = this._chunksHorizontalLines[i];
                fe.Dispatcher.Invoke(() => e.DrawingContext.DrawRectangle(rr.Brush, null, rr.Rect));
            }

            System.Diagnostics.Debug.WriteLine("_chunkLinesLayer_Rendering complete");
        }

        private void _pixelMapLayer_Rendering(object sender, HandleableElement.HandleableElementRenderingEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("_pixelMapLayer_Rendering start");

            // Dispatcher 経由で改善なし
            var fe = (FrameworkElement)sender;
            for (var i = 0; i < this._pixelWidth; i++)
            {
                for (var j = 0; j < this._pixelHeight; j++)
                {
                    var rr = this._pixRectangels[i][j];
                    try
                    {
                        if (i % 500 == 0)
                            System.Diagnostics.Debug.WriteLine("_pixelMapLayer_Rendering pix exec");
                        fe.Dispatcher.Invoke(() =>
                        {
                            try
                            {
                                e.DrawingContext.DrawRectangle(rr.Brush, null, rr.Rect);
                            }
                            catch (Exception ex)
                            {
                                System.Diagnostics.Debug.WriteLine("_pixelMapLayer_Rendering error!!");
                                throw ex;
                            }
                        });
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine("_pixelMapLayer_Rendering error!! (External)");
                        throw ex;
                    }
                }
            }

            System.Diagnostics.Debug.WriteLine("_pixelMapLayer_Rendering complete");
        }
    }
}
