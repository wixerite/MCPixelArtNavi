using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using Microsoft.Win32;

using Prism.Commands;
using Prism.Mvvm;

using MCPArtNavi.Common;
using MCPArtNavi.Common.PxartFileUtils;

namespace MCPArtNavi.UserApp
{
    public class MainWindowViewModel : BindableBase
    {
        // 非公開フィールド


        // 公開プロパティ


        // バインディング プロパティ

        private PixelCanvasViewModel _canvasViewModel;

        public PixelCanvasViewModel CanvasViewModel
        {
            get => this._canvasViewModel;
            set => this.SetProperty(ref this._canvasViewModel, value);
        }

        private Visibility _canvasVisility;

        public Visibility CanvasVisility
        {
            get => this._canvasVisility;
            set => this.SetProperty(ref this._canvasVisility, value);
        }

        private Visibility _loadingTextVisility;

        public Visibility LoadingTextVisility
        {
            get => this._loadingTextVisility;
            set => this.SetProperty(ref this._loadingTextVisility, value);
        }

        private double _canvasZoom;

        public double CanvasZoom
        {
            get => this._canvasZoom;
            set => this.SetProperty(ref this._canvasZoom, value);
        }


        // コマンド

        public DelegateCommand SaveAsCommand
        {
            get => new DelegateCommand(this._saveAs_command);
        }

        public DelegateCommand OpenCommand
        {
            get => new DelegateCommand(this._open_command);
        }

        public DelegateCommand LoadExampleImageCommand
        {
            get => new DelegateCommand(this._debug_loadExampleArt);
        }


        // コンストラクタ

        public MainWindowViewModel()
        {
            this.CanvasViewModel = new PixelCanvasViewModel();
            this.CanvasZoom = 4.0d;

            this.CanvasVisility = Visibility.Visible;
            this.LoadingTextVisility = Visibility.Collapsed;
        }


        // 非公開メソッド

        private void _open_command()
        {
            var openFileDialog = new OpenFileDialog()
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Filter = "MC Pixel Art Navi Document (*.mcpxart)|*.mcpxart|All Files (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                PixelArtDocument doc = null;
                using (var fs = File.OpenRead(openFileDialog.FileName))
                {
                    doc = PixelArtFile.LoadFrom(fs);
                }

                this.CanvasVisility = Visibility.Hidden;
                this.LoadingTextVisility = Visibility.Visible;
                this.CanvasViewModel.LoadPixelArt(doc);
                this.CanvasVisility = Visibility.Visible;
                this.LoadingTextVisility = Visibility.Collapsed;
            }
        }

        private void _saveAs_command()
        {
            var saveFileDialog = new SaveFileDialog()
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                FileName = "pixelart.mcpxart",
                Filter = "MC Pixel Art Navi Document (*.mcpxart)|*.mcpxart|All Files (*.*)|*.*"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                var doc = this.CanvasViewModel.GetPixelArt();
                using (var fs = File.OpenWrite(saveFileDialog.FileName))
                {
                    PixelArtFile.SaveTo(fs, doc);
                }
            }
        }

        private void _debug_loadExampleArt()
        {
            // Example
            this.CanvasVisility = Visibility.Hidden;
            this.LoadingTextVisility = Visibility.Visible;

            var white_wool = new Common.Items.MCWhiteWool();
            var black_wool = new Common.Items.MCBlackWool();

            var pxartDoc = new PixelArtDocument();
            pxartDoc.Size = PixelArtSize.Size128x128;
            pxartDoc.Pixels = new IMCItem[pxartDoc.Size.GetWidth() * pxartDoc.Size.GetHeight()];
            for (var i = 0; i < pxartDoc.Pixels.Length; i++)
            {
                if (i % 3 == 0)
                    pxartDoc.Pixels[i] = white_wool;
                else
                    pxartDoc.Pixels[i] = black_wool;
            }

            this.CanvasViewModel.LoadPixelArt(pxartDoc);

            this.CanvasVisility = Visibility.Visible;
            this.LoadingTextVisility = Visibility.Collapsed;
        }
    }
}
