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
            // 一番最初のアイテムが起動時の空ドキュメントの fill として利用されます
            EnabledItems = new List<IMCItem>()
            {
                new MCSand(),
                new MCBlackWool(),
                new MCWhiteWool(),
                new MCGreenWool(),
                new MCLimeWool(),
            };
        }


        // 公開静的メソッド

        public static IMCItem GetItemById(string itemId)
        {
            return EnabledItems.Single(item => item.ItemId == itemId);
        }
    }
}
