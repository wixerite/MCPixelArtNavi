using System;
using System.Collections.Generic;
using System.Text;

namespace MCPArtNavi.Common.Items
{
    public class MCLightGrayWool : MCWoolTypeBase
    {
        // 公開プロパティ

        public override string ItemId
        {
            get => "light_gray_wool";
        }

        public override string ItemColor
        {
            get => "#84898C";
        }
    }
}
