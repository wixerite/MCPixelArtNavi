using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCPArtNavi.Common.Items
{
    public static class MCItemUtils
    {
        // 公開プロパティ

        public static IReadOnlyCollection<IMCItem> EnabledItems
        {
            get;
        }


        // コンストラクタ

        static MCItemUtils()
        {
            EnabledItems = new List<IMCItem>()
            {
                new MCBlackWool(),
                new MCWhiteWool(),
            };
        }


        // 公開静的メソッド

        public static IMCItem GetItemById(string itemId)
        {
            return EnabledItems.Single(item => item.ItemId == itemId);
        }
    }
}
