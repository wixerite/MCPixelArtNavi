using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

using MCPArtNavi.Common;

namespace MCPArtNavi.UserApp.ImportWindowInternal
{
    public class ImporterMCItem
    {
        // 公開プロパティ

        public IMCItem Item
        {
            get;
            set;
        }

        public bool Use
        {
            get;
            set;
        }

        public Color ItemColor
        {
            get => (Color)ColorConverter.ConvertFromString(this.Item.ItemColor);
        }

        public Brush ItemBrush
        {
            get
            {
                var brush = new SolidColorBrush(this.ItemColor);
                brush.Freeze();
                return brush;
            }
        }
    }
}
