using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

using MCPArtNavi.Common;

namespace MCPArtNavi.UserApp.PixelCanvasInternal
{
    public class PaletteItem
    {
        public string ColorCode
        {
            get => this.MCItem.ItemColor;
        }

        public Color Color
        {
            get;
            private set;
        }

        public Brush Brush
        {
            get;
            private set;
        }

        public IMCItem MCItem
        {
            get;
            private set;
        }


        // コンストラクタ

        /// <summary>
        /// <see cref="PaletteItem"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="color"></param>
        /// <param name="brush"></param>
        /// <param name="pen"></param>
        /// <param name="mcItem"></param>
        private PaletteItem(Color color, Brush brush, IMCItem mcItem)
        {
            this.Color = color;
            this.Brush = brush;
            this.MCItem = mcItem;
        }


        // 公開静的メソッド

        public static PaletteItem CreateFrom(IMCItem mcItem)
        {
            var color = (Color)ColorConverter.ConvertFromString(mcItem.ItemColor);
            var brush = new SolidColorBrush(color);

            return new PaletteItem(color, brush, mcItem);
        }
    }
}
