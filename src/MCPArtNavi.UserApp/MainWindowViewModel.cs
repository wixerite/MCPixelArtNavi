using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Prism.Mvvm;

namespace MCPArtNavi.UserApp
{
    public class MainWindowViewModel : BindableBase
    {
        // 非公開フィールド


        // 公開プロパティ


        // バインディング プロパティ

        private PixelCanvasViewModel _pixelCanvasViewModel;

        public PixelCanvasViewModel PixelCanvasDataContext
        {
            get => this._pixelCanvasViewModel;
            set => this.SetProperty(ref this._pixelCanvasViewModel, value);
        }
    }
}
