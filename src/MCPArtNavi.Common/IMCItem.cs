using System;
using System.Collections.Generic;
using System.Text;

namespace MCPArtNavi.Common
{
    public interface IMCItem
    {
        string ItemId
        {
            get;
        }

        string ItemName
        {
            get;
        }

        string ItemColor
        {
            get;
        }
    }
}
