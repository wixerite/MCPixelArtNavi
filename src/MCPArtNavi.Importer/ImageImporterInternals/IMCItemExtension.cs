using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MCPArtNavi.Common;

namespace MCPArtNavi.Importer.ImageImporterInternals
{
    internal static class IMCItemExtension
    {
        public static Color GetColor(this IMCItem item)
        {
            return ColorTranslator.FromHtml(item.ItemColor);
        }
    }
}
