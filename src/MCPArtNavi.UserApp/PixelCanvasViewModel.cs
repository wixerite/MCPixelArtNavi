using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

using Prism.Mvvm;

using MCPArtNavi.Common;
using MCPArtNavi.Common.Items;
using MCPArtNavi.UserApp.PixelCanvasInternal;
using System.Windows.Media.Imaging;

namespace MCPArtNavi.UserApp
{
    public class PixelCanvasViewModel : BindableBase
    {
        // 非公開フィールド
        private Palette _palette;

        private EventHandler<ItemMouseEventArgs> _itemMouseMove;
        private EventHandler<ItemMouseEventArgs> _itemMouseDown;

        private bool _isDrag;


        // 公開プロパティ

        public IMCItem PenItem
        {
            get;
            set;
        }


        // 公開イベント

        public event EventHandler<ItemMouseEventArgs> ItemMouseMove
        {
            add => this._itemMouseMove += value;
            remove => this._itemMouseMove -= value;
        }

        public event EventHandler<ItemMouseEventArgs> ItemMouseDown
        {
            add => this._itemMouseDown += value;
            remove => this._itemMouseDown -= value;
        }


        // バインディング プロパティ

        private int _pixelArtWidth;
        public int PixelArtWidth
        {
            get => this._pixelArtWidth;
            set => this.SetProperty(ref this._pixelArtWidth, value);
        }

        private int _pixelArtHeight;
        public int PixelArtHeight
        {
            get => this._pixelArtHeight;
            set => this.SetProperty(ref this._pixelArtHeight, value);
        }

        private PixelCanvasMapHandler _mapHandler;

        /// <summary>
        /// バインディング プロパティです。<see cref="PixelCanvasMapHandler"/> を取得または設定します。バインディング目的以外での外部からの利用禁止。
        /// </summary>
        public PixelCanvasMapHandler MapHandler
        {
            get => this._mapHandler;
            set => this.SetProperty(ref this._mapHandler, value);
        }


        // コンストラクタ

        public PixelCanvasViewModel()
        {
            this.PixelArtWidth = 16;
            this.PixelArtHeight = 16;

            this._palette = new Palette();
            foreach (var e in MCItemUtils.EnabledItems)
                this._palette.Items.Add(PaletteItem.CreateFrom(e));

            this.MapHandler = new PixelCanvasMapHandler();
            this.MapHandler.CanvasMouseMove += this._mapHandler_CanvasMouseMove;
            this.MapHandler.CanvasMouseDown += this._mapHandler_CanvasMouseDown;
            this.MapHandler.CanvasMouseUp += this._mapHandler_CanvasMouseUp;
        }


        // 非公開メソッド

        private void _mapHandler_CanvasMouseMove(object sender, PixelCanvasMapHandler.PixelMouseEventArgs e)
        {
            if (!e.IsPixelHit)
                return;

            this._itemMouseMove?.Invoke(this, new ItemMouseEventArgs()
            {
                Item = this._palette.GetByBrush(e.PixelBrush).MCItem,
                X = e.X,
                Y = e.Y,
            });

            if (this._isDrag == false || this.PenItem == null)
                return;
            this.MapHandler.SetPixel(e.X, e.Y, this._palette.GetByMCItem(this.PenItem).Brush);
            //if (Environment.TickCount % 50 == 0)
                this.MapHandler.RedrawLayout();
        }

        private void _mapHandler_CanvasMouseDown(object sender, PixelCanvasMapHandler.PixelMouseEventArgs e)
        {
            this._isDrag = true;

            if (!e.IsPixelHit)
                return;

            this._itemMouseDown?.Invoke(this, new ItemMouseEventArgs()
            {
                Item = this._palette.GetByBrush(e.PixelBrush).MCItem,
                X = e.X,
                Y = e.Y,
            });
        }

        private void _mapHandler_CanvasMouseUp(object sender, PixelCanvasMapHandler.PixelMouseEventArgs e)
        {
            //this._itemMouseDown?.Invoke(this, new ItemMouseEventArgs()
            //{
            //    Item = this._palette.GetByBrush(e.PixelBrush).MCItem,
            //    X = e.X,
            //    Y = e.Y,
            //});

            this._isDrag = false;
            if (this.PenItem == null || !e.IsPixelHit)
                return;
            this.MapHandler.SetPixel(e.X, e.Y, this._palette.GetByMCItem(this.PenItem).Brush);
            this.MapHandler.RedrawLayout();
        }


        // 公開メソッド

        public void LoadPixelArt(PixelArtDocument document)
        {
            this.MapHandler.InvokeDispatcher(() =>
            {
                // RedrawLayout の処理が走る前に PixelCanvas に対してサイズ変更を反映させる
                this.PixelArtWidth = document.Size.GetWidth();
                this.PixelArtHeight = document.Size.GetHeight();
            });

            var p = 0;
            for (var i = 0; i < this.PixelArtHeight; i++)
            {
                for (var j = 0; j < this.PixelArtWidth; j++, p++)
                {
                    var item = document.Pixels[p];
                    //if (this._palette.ContainsMCItem(item) == false)
                    //    this._palette.Items.Add(PaletteItem.CreateFrom(item));

                    this.MapHandler.SetPixel(j, i, this._palette.GetByMCItem(item).Brush);
                }
            }

            this.MapHandler.RedrawLayout();
        }

        public PixelArtDocument GetPixelArt()
        {
            var artSize = default(PixelArtSize);
            foreach (var s in Enum.GetValues(typeof(PixelArtSize)).Cast<PixelArtSize>())
            {
                if (s.GetWidth() != this.PixelArtWidth || s.GetHeight() != this.PixelArtHeight)
                    continue;

                artSize = s;
                break;
            }

            var colorTable = new Dictionary<Color, IMCItem>();
            foreach (var item in MCItemUtils.EnabledItems)
                colorTable[(Color)ColorConverter.ConvertFromString(item.ItemColor)] = item;

            var pixels = new IMCItem[this.PixelArtWidth * this.PixelArtHeight];
            var p = 0;
            for (var i = 0; i < this.PixelArtHeight; i++)
            {
                for (var j = 0; j < this.PixelArtWidth; j++, p++)
                {
                    pixels[p] = this._palette.GetByBrush(this.MapHandler.GetPixel(j, i)).MCItem;
                }
            }

            var document = new PixelArtDocument();
            document.DocumentTitle = "";
            document.DocumentAuthor = "";
            document.DocumentDescription = "";
            document.Size = artSize;
            document.Pixels = pixels;
            
            return document;
        }

        public BitmapSource CanvasToBitmap()
        {
            return this.MapHandler.CanvasToBitmap();
        }

        
        // その他

        public class ItemMouseEventArgs
        {
            public IMCItem Item
            {
                get;
                set;
            }

            public int X
            {
                get;
                set;
            }

            public int Y
            {
                get;
                set;
            }
        }
    }
}
