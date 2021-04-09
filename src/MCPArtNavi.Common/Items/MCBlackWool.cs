using System;
using System.Collections.Generic;
using System.Text;

namespace MCPArtNavi.Common.Items
{
    public class MCBlackWool : MCWoolTypeBase
    {
        // 公開プロパティ

        public override string ItemId
        {
            get => "black_wool";
        }

        public override string ItemColor
        {
            get => "#050505";
        }
    }
}
