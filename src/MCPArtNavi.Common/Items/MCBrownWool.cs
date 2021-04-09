using System;
using System.Collections.Generic;
using System.Text;

namespace MCPArtNavi.Common.Items
{
    public class MCBrownWool : MCWoolTypeBase
    {
        // 公開プロパティ

        public override string ItemId
        {
            get => "brown_wool";
        }

        public override string ItemColor
        {
            get => "#7A4B22";
        }
    }
}
