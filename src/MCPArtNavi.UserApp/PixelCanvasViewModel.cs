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

namespace MCPArtNavi.UserApp
{
    public class PixelCanvasViewModel : BindableBase
    {
        // 非公開フィールド
        private Palette _palette;


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
            this.PixelArtWidth = 128;
            this.PixelArtHeight = 128;

            this.MapHandler = new PixelCanvasMapHandler();

            this._palette = new Palette();
        }


        // 公開メソッド

        public void LoadPixelArt(PixelArtDocument document)
        {
            this.PixelArtWidth = document.Size.GetWidth();
            this.PixelArtHeight = document.Size.GetHeight();

            var p = 0;
            for (var i = 0; i < this.PixelArtHeight; i++)
            {
                for (var j = 0; j < this.PixelArtWidth; j++, p++)
                {
                    var item = document.Pixels[p];
                    if (this._palette.ContainsMCItem(item) == false)
                        this._palette.Items.Add(PaletteItem.CreateFrom(item));

                    this.MapHandler.SetPixel(j, i, this._palette.GetByMCItem(item).Brush);
                }
            }

            this.MapHandler.RedrawPixels();
        }

        public PixelArtDocument GetPixelArt()
        {
            var artSize = default(PixelArtSize);
            foreach (var s in Enum.GetValues(typeof(PixelArtSize)).Cast<PixelArtSize>())
            {
                if (s.GetWidth() != this.PixelArtWidth && s.GetHeight() != this.PixelArtHeight)
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
            document.DocumentTitle = "Untitled Map";
            document.Size = artSize;
            document.Pixels = pixels;
            
            return document;
        }
    }
}
