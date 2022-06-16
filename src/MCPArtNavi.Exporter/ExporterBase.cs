using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MCPArtNavi.Common;

namespace MCPArtNavi.Exporter
{
    public abstract class ExporterBase
    {
        // 公開プロパティ

        public abstract ExportOption Option
        {
            get;
            set;
        }

        public abstract Task<ExportResult> ExportAsync(PixelArtDocument document, Stream stream, string baseDirectory);
    }
}
