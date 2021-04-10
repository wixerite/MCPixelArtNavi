using System;
using System.Collections.Generic;
using System.Text;

namespace MCPArtNavi.Common.Items
{
    public class MCGreenWool : MCWoolTypeBase
    {
        // 公開プロパティ

        public override string ItemId
        {
            get => "green_wool";
        }

        public override string ItemColor
        {
            get => "#5E820C";
        }
    }
}
