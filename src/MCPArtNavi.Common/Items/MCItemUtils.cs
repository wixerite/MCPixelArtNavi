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

                // 羊毛
                new MCWhiteWool(),
                new MCOrangeWool(),
                new MCMagentaWool(),
                new MCLightBlueWool(),
                new MCYellowWool(),
                new MCLimeWool(),
                new MCPinkWool(),
                new MCGrayWool(),
                new MCLightGrayWool(),
                new MCCyanWool(),
                new MCPurpleWool(),
                new MCBlueWool(),
                new MCBrownWool(),
                new MCGreenWool(),
                new MCRedWool(),
                new MCBlackWool(),
            };
        }


        // 公開静的メソッド

        public static IMCItem GetItemById(string itemId)
        {
            return EnabledItems.Single(item => item.ItemId == itemId);
        }
    }
}
