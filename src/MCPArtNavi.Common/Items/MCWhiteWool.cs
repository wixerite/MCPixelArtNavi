using System;
using System.Collections.Generic;
using System.Text;

namespace MCPArtNavi.Common.Items
{
    public class MCWhiteWool : MCWoolTypeBase
    {
        // 公開プロパティ

        public override string ItemId
        {
            get => "white_wool";
        }

        public override string ItemColor
        {
            get => "#FAFAFA";
        }
    }
}
