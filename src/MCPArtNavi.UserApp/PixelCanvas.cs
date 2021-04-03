using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MCPArtNavi.UserApp
{
    public class PixelCanvas : UserControl
    {
        // 非公開フィールド
        private Brush defaultColorBrush;


        // 公開プロパティ


        // コンストラクタ

        public PixelCanvas()
        {
            this.defaultColorBrush = new SolidColorBrush(Colors.Silver);
            this._initializeComponent();
        }


        // 非公開メソッド

        private void _initializeComponent()
        {
            var grid = new Grid();
            for (var i = 0; i < 10; i++)
            {
                grid.RowDefinitions.Add(new RowDefinition());
                grid.ColumnDefinitions.Add(new ColumnDefinition());
            }

            for (var i = 0; i < 10; i++)
            {
                for (var j = 0; j < 10; j++)
                {
                    var pixRect = new Rectangle();
                    pixRect.SetValue(Grid.RowProperty, i);
                    pixRect.SetValue(Grid.ColumnProperty, i);
                    pixRect.Fill = this.defaultColorBrush;

                    grid.Children.Add(pixRect);
                }
            }

            this.Content = grid;
        }
    }
}
