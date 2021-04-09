using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.ComponentModel;
using System.Windows.Media.Imaging;

using Microsoft.Win32;

using Prism.Commands;
using Prism.Mvvm;

using MCPArtNavi.Common;
using MCPArtNavi.Common.Items;
using MCPArtNavi.Common.PxartFileUtils;
using MCPArtNavi.Importer;

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

        private Visibility _canvasVisibility;

        public Visibility CanvasVisibility
        {
            get => this._canvasVisibility;
            set => this.SetProperty(ref this._canvasVisibility, value);
        }

        private Visibility _loadingTextVisibility;

        public Visibility LoadingTextVisibility
        {
            get => this._loadingTextVisibility;
            set => this.SetProperty(ref this._loadingTextVisibility, value);
        }

        private double _canvasZoom;

        public double CanvasZoom
        {
            get => this._canvasZoom;
            set => this.SetProperty(ref this._canvasZoom, value);
        }

        private bool _showChunkLinesChecked;

        public bool ShowChunkLinesChecked
        {
            get => this._showChunkLinesChecked;
            set => this.SetProperty(ref this._showChunkLinesChecked, value);
        }

        private Visibility _chunkLinesVisibility;

        public Visibility ChunkLinesVisibility
        {
            get => this._chunkLinesVisibility;
            set => this.SetProperty(ref this._chunkLinesVisibility, value);
        }

        private bool _showToolPanelChecked;

        public bool ShowToolPanelChecked
        {
            get => this._showToolPanelChecked;
            set => this.SetProperty(ref this._showToolPanelChecked, value);
        }
        
        private Visibility _toolPanelVisibility;

        public Visibility ToolPanelVisivility
        {
            get => this._toolPanelVisibility;
            set => this.SetProperty(ref this._toolPanelVisibility, value);
        }

        private string _onMouseItemNameText;
        public string OnMouseItemNameText
        {
            get => this._onMouseItemNameText;
            set => this.SetProperty(ref this._onMouseItemNameText, value);
        }


        // コマンド

        public DelegateCommand SaveAsCommand
        {
            get => new DelegateCommand(this._saveAs_command);
        }

        public DelegateCommand OpenCommand
        {
            get => new DelegateCommand(async () => await this._open_commandAsync());
        }

        public DelegateCommand ImportCommand
        {
            get => new DelegateCommand(this._import_command);
        }

        public DelegateCommand ExportCommand
        {
            get => new DelegateCommand(this._export_command);
        }

        public DelegateCommand LoadExampleImageCommand
        {
            get => new DelegateCommand(this._debug_loadExampleArt);
        }


        // コンストラクタ

        public MainWindowViewModel()
        {

            this.CanvasVisibility = Visibility.Visible;
            this.LoadingTextVisibility = Visibility.Collapsed;
            this.ShowChunkLinesChecked = true;
            this.ShowToolPanelChecked = true;

            App.Current.MainWindow.Loaded += (sender, e) =>
            {
                this.CanvasViewModel.LoadPixelArt(PixelArtDocument.GetEmptyDocument(PixelArtSize.Size128x128, MCItemUtils.EnabledItems.First()));
            };

            this.CanvasViewModel = new PixelCanvasViewModel();
            this.CanvasViewModel.ItemMouseMove += (sender, e) => this.OnMouseItemNameText = e.Item.ItemName;
            this.CanvasZoom = 4.0d;
        }


        // 限定公開メソッド

        protected override void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            // 基底呼び出し
            base.OnPropertyChanged(args);

            switch (args.PropertyName)
            {
                case nameof(ShowChunkLinesChecked):
                    if (this.ShowChunkLinesChecked)
                        this.ChunkLinesVisibility = Visibility.Visible;
                    else
                        this.ChunkLinesVisibility = Visibility.Hidden;
                    break;
                case nameof(ShowToolPanelChecked):
                    if (this.ShowToolPanelChecked)
                        this.ToolPanelVisivility = Visibility.Visible;
                    else
                        this.ToolPanelVisivility = Visibility.Collapsed;
                    break;
            }
        }


        // 非公開メソッド

        private async Task _open_commandAsync()
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

                
                System.Diagnostics.Debug.WriteLine("Start load doc (external)");

                await Task.Run(() =>
                {
                    this.CanvasVisibility = Visibility.Hidden;
                    this.LoadingTextVisibility = Visibility.Visible;

                    System.Diagnostics.Debug.WriteLine("Start load doc");
                    this.CanvasViewModel.LoadPixelArt(doc);
                    System.Diagnostics.Debug.WriteLine("Complete load doc");

                    this.CanvasVisibility = Visibility.Visible;
                    this.LoadingTextVisibility = Visibility.Collapsed;
                });

                System.Diagnostics.Debug.WriteLine("Complete load doc (external)");
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

        private void _import_command()
        {
            var w = new ImportWindow();
            w.ShowDialog();

            var doc = ((ImportWindowViewModel)w.DataContext).ResultDocument;
            if (doc != null)
                this.CanvasViewModel.LoadPixelArt(doc);
        }

        private void _export_command()
        {
            var saveFileDialog = new SaveFileDialog()
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                FileName = "export.png",
                Filter = "PNG Image (*.png)|*.png|All Files (*.*)|*.*"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                using (var fs = File.OpenWrite(saveFileDialog.FileName))
                {
                    var encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(this.CanvasViewModel.CanvasToBitmap()));
                    encoder.Save(fs);
                }
            }
        }

        private void _debug_loadExampleArt()
        {
            // Example
            this.CanvasVisibility = Visibility.Hidden;
            this.LoadingTextVisibility = Visibility.Visible;

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

            this.CanvasVisibility = Visibility.Visible;
            this.LoadingTextVisibility = Visibility.Collapsed;
        }
    }
}
