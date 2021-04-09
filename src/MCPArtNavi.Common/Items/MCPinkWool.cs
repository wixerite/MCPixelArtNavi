using System;
using System.Collections.Generic;
using System.Text;

namespace MCPArtNavi.Common.Items
{
    public class MCPinkWool : MCWoolTypeBase
    {
        // 公開プロパティ

        public override string ItemId
        {
            get => "pink_wool";
        }

        public override string ItemColor
        {
            get => "#FC9EBE";
        }
    }
}
