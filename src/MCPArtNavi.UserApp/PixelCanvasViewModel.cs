using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

using Prism.Mvvm;

namespace MCPArtNavi.UserApp
{
    public class PixelCanvasViewModel : BindableBase
    {
        // 非公開フィールド
        

        // 公開プロパティ


        // バインディング プロパティ

        private ObservableCollection<ObservableCollection<Brush>> _pixelMap;

        public ObservableCollection<ObservableCollection<Brush>> PixelMap
        {
            get => this._pixelMap;
            set => this.SetProperty(ref this._pixelMap, value);
        }


        // コンストラクタ

        public PixelCanvasViewModel()
        {
            this.PixelMap = new ObservableCollection<ObservableCollection<Brush>>();
            this.PixelMap.Add(new ObservableCollection<Brush>());
            this.PixelMap[0].Add(new SolidColorBrush(Colors.Red));
        }


        // 公開メソッド

        public void SetPixelMap(Brush[] palette, int[][] map)
        {
            this.PixelMap = new ObservableCollection<ObservableCollection<Brush>>();
            if (map.GetLength(0) != 512 || map.GetLength(1) != 512)
                throw new InvalidOperationException();

            for (var i = 0; i < 512; i++)
            {
                this.PixelMap.Add(new ObservableCollection<Brush>());
                for (var j = 0; j < 512; j++)
                    this.PixelMap[i][j] = palette[map[i][j]];
            }
        }
    }
}
