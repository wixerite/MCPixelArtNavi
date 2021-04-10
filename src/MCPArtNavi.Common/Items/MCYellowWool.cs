using System;
using System.Collections.Generic;
using System.Text;

namespace MCPArtNavi.Common.Items
{
    public class MCYellowWool : MCWoolTypeBase
    {
        // 公開プロパティ

        public override string ItemId
        {
            get => "yellow_wool";
        }

        public override string ItemColor
        {
            get => "#F0C01A";
        }
    }
}
