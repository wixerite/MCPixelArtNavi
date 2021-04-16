using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MCPArtNavi.Common;
using MCPArtNavi.Common.Items;
using MCPArtNavi.Exporter.OpenXML;

namespace MCPArtNavi.Exporter.Check
{
    class Program
    {
        static void Main(string[] args)
        {
            var exporter = new SpreadSheetExporter();
            using (var s = new MemoryStream())
            {
                var r = exporter.ExportAsync(PixelArtDocument.GetEmptyDocument(PixelArtSize.Size128x128, MCItemUtils.EnabledItems.First()), s).Result;
                Console.WriteLine(s.Length);

                using (var bw = new BinaryWriter(File.OpenWrite("test.xlsx")))
                {
                    bw.Write(s.ToArray());
                }
            }
        }
    }
}
