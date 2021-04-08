using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MCPArtNavi.Importer.ImageImporterInternals;

namespace MCPArtNavi.Importer
{
    public static class ForCheck
    {
        public static double GetXyzDiff(Color c1, Color c2)
        {
            return XyzDifference.Difference(c1, c2);
        }
    }
}
