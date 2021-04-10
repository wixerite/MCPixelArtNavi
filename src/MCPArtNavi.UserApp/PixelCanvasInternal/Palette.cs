using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

using MCPArtNavi.Common;

namespace MCPArtNavi.UserApp.PixelCanvasInternal
{
    public class Palette
    {
        // 公開プロパティ

        public IList<PaletteItem> Items
        {
            get;
            private set;
        }

        public int Count
        {
            get => this.Items.Count;
        }


        // コンストラクタ

        public Palette()
        {
            this.Items = new List<PaletteItem>();
        }


        // 公開メソッド

        public PaletteItem GetByColorCode(string colorCode)
        {
            return this.Items.First(e => e.ColorCode == colorCode);
        }

        public PaletteItem GetByColor(Color color)
        {
            return this.Items.First(e => e.Color.Equals(color));
        }

        public PaletteItem GetByBrush(Brush brush)
        {
            return this.Items.First(e => e.Brush == brush);
        }

        public PaletteItem GetByMCItem(IMCItem mcItem)
        {
            return this.Items.First(e => e.MCItem == mcItem);
        }

        public bool ContainsMCItem(IMCItem mcItem)
        {
            return this.Items.Any(e => e.MCItem == mcItem);
        }
    }
}
