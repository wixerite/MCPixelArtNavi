using System;
using System.Collections.Generic;
using System.Text;

namespace MCPArtNavi.Common.Items
{
    public abstract class MCItemBase : IMCItem
    {
        // 公開プロパティ

        public abstract string ItemId
        {
            get;
        }

        public string ItemName
        {
            get => this._getItemName();
        }

        public abstract string ItemColor
        {
            get;
        }


        // 非公開メソッド

        private string _getItemName()
        {
            var resourceName = $"ItemName_{this.ItemId}";

            try
            {
                return Properties.Resources.ResourceManager.GetString(resourceName);
            }
            catch
            {
                System.Diagnostics.Debug.WriteLine("Failure to get string: {0}", resourceName);
            }

            return "Unknown";
        }
    }
}
