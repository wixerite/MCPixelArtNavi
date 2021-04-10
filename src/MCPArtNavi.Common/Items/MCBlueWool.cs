using System;
using System.Collections.Generic;
using System.Text;

namespace MCPArtNavi.Common.Items
{
    public class MCBlueWool : MCWoolTypeBase
    {
        // 公開プロパティ

        public override string ItemId
        {
            get => "blue_wool";
        }

        public override string ItemColor
        {
            get => "#1D489B";
        }
    }
}
