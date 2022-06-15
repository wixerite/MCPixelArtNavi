using MCPArtNavi.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCPArtNavi.Exporter.CreativeCommand
{
    public class CreativeCommandExporter : ExporterBase
    {
        public override ExportOption Option
        {
            get;
            set;
        }

        public override async Task<ExportResult> ExportAsync(PixelArtDocument document, Stream stream)
        {
            var sw = new StreamWriter(stream);
            IMCItem currentItem = document.Pixels[0];

            for (var row = 0; row < document.Size.GetHeight(); row++)
            {
                for (var col = 0; col < document.Size.GetWidth(); col++)
                {
                    var nextItem = document.Pixels[row + col];
                    if (currentItem != nextItem)
                    {
                        // コマンドの出力

                        // currentItem の変更
                        currentItem = nextItem;
                    }
                }
            }

            await sw.FlushAsync();
            return new ExportResult();
        }
    }
}
