using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MCPArtNavi.Common;

namespace MCPArtNavi.Importer
{
    public class ImporterPalette
    {
        // 非公開フィールド


        // 公開プロパティ

        public List<IMCItem> Items
        {
            get;
            private set;
        }


        // コンストラクタ

        public ImporterPalette()
        {
            this.Items = new List<IMCItem>();
        }
    }
}
