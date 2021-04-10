using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using MCPArtNavi.UserApp.MainWindowInternal;

namespace MCPArtNavi.UserApp
{
    public class MainWindowViewModel : BindableBase
    {
        // 非公開フィールド

        // 公開プロパティ

        /// <summary>
        /// 現在編集中のファイルのパスを取得または設定します。
        /// 未保存のファイルの場合、null を示します。
        /// </summary>
        public string DocumentFilePath
        {
            get;
            private set;
        }


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

        private string _bottomHintText;
        public string BottomHintText
        {
            get => this._bottomHintText;
            set => this.SetProperty(ref this._bottomHintText, value);
        }

        private ObservableCollection<AvailableMCItem> _availableMCItems;
        public ObservableCollection<AvailableMCItem> AvailableMCItems
        {
            get => this._availableMCItems;
            set => this.SetProperty(ref this._availableMCItems, value);
        }

        private AvailableMCItem _selectedToolMCItem;
        public AvailableMCItem SelectedToolMCItem
        {
            get => this._selectedToolMCItem;
            set => this.SetProperty(ref this._selectedToolMCItem, value);
        }

        private PixelArtDocumentMetadata _currentDocumentMetadata;
        public PixelArtDocumentMetadata CurrentDocumentMetadata
        {
            get => this._currentDocumentMetadata;
            set => this.SetProperty(ref this._currentDocumentMetadata, value);
        }

        private string _windowTitle;
        public string WindowTitle
        {
            get => this._windowTitle;
            set => this.SetProperty(ref this._windowTitle, value);
        }

        private bool _documentChanged;
        public bool DocumentChanged
        {
            get => this._documentChanged;
            set => this.SetProperty(ref this._documentChanged, value);
        }


        // コマンド

        public DelegateCommand SaveCommand
        {
            get => new DelegateCommand(this._save_command);
        }

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

        public DelegateCommand EditDocumentPropertyCommand
        {
            get => new DelegateCommand(this._editDocumentProperty_command);
        }

        public DelegateCommand<CancelEventArgs> WindowClosingCommand
        {
            get => new DelegateCommand<CancelEventArgs>(this._windowClosing);
        }


        // コンストラクタ

        public MainWindowViewModel()
        {
            this.CanvasVisibility = Visibility.Visible;
            this.LoadingTextVisibility = Visibility.Collapsed;
            this.ShowChunkLinesChecked = true;
            this.ShowToolPanelChecked = false;
            this.ToolPanelVisivility = Visibility.Collapsed;
            this.AvailableMCItems = new ObservableCollection<AvailableMCItem>(MCItemUtils.EnabledItems.Select(e => new AvailableMCItem() { Item = e }));
            this.WindowTitle = Properties.Resources.ApplicationTitle;

            if (App.Current != null && App.Current.MainWindow != null)
            {
                // デザイナではない
                App.Current.MainWindow.Loaded += (sender, e) =>
                {
                    this.CanvasViewModel.LoadPixelArt(PixelArtDocument.GetEmptyDocument(PixelArtSize.Size128x128, MCItemUtils.EnabledItems.First()));
                    this.CurrentDocumentMetadata = new PixelArtDocumentMetadata()
                    {
                        DocumentTitle = "Untitled",
                        DocumentAuthor = "",
                        DocumentDescription = "",
                    };
                };
            }

            this.CanvasViewModel = new PixelCanvasViewModel();
            this.CanvasViewModel.ItemMouseMove += _canvasViewModel_ItemMouseMove;
            this.CanvasViewModel.CanvasEdit += (sender, e) => this.DocumentChanged = true;
            this.CanvasZoom = 5.0d;
        }

        private void _canvasViewModel_ItemMouseMove(object sender, PixelCanvasViewModel.ItemMouseEventArgs e)
        {
            int nonProgrammersX = e.X + 1;
            int nonProgrammersY = e.Y + 1;

            int nonProgrammersXinChunk = e.X % 16 + 1;
            int nonProgrammersYinChunk = e.Y % 16 + 1;

            int nonProgrammersChunkX = e.X / 16 + 1;
            int nonProgrammersChunkY = e.Y / 16 + 1;

            this.BottomHintText =
                $"Global: {nonProgrammersX.ToString()}, {nonProgrammersY.ToString()} / " +
                $"In chunk: {nonProgrammersXinChunk.ToString()}, {nonProgrammersYinChunk.ToString()} / " +
                $"Chunks: {nonProgrammersChunkX}, {nonProgrammersChunkY} / Item: {e.Item.ItemName}";
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
                    {
                        this.ToolPanelVisivility = Visibility.Visible;
                        if (this.CanvasViewModel != null)
                            this.CanvasViewModel.PenItem = this.SelectedToolMCItem?.Item;
                    }
                    else
                    {
                        this.ToolPanelVisivility = Visibility.Collapsed;
                        if (this.CanvasViewModel != null)
                            this.CanvasViewModel.PenItem = null;
                    }
                    break;
                case nameof(SelectedToolMCItem):
                    if (this.CanvasViewModel != null)
                        this.CanvasViewModel.PenItem = this.SelectedToolMCItem?.Item;
                    if (this.SelectedToolMCItem == null)
                        return;
                    this.BottomHintText = $"Selected '{this.SelectedToolMCItem.Item.ItemName}'.";
                    break;
                case nameof(CurrentDocumentMetadata):
                case nameof(DocumentChanged):
                    if (this.CurrentDocumentMetadata == null)
                    {
                        this.WindowTitle = Properties.Resources.ApplicationTitle;
                        return;
                    }

                    var text = this.CurrentDocumentMetadata.DocumentTitle;
                    if (String.IsNullOrEmpty(text) && !String.IsNullOrEmpty(this.DocumentFilePath))
                        text = Path.GetFileName(this.DocumentFilePath);

                    if (this._documentChanged)
                        text += " (*)";

                    text += " - " + Properties.Resources.ApplicationTitle;

                    this.WindowTitle = text;
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

                this.DocumentFilePath = openFileDialog.FileName;
                this.CurrentDocumentMetadata = doc;

                await Task.Run(() =>
                {
                    this.CanvasVisibility = Visibility.Hidden;
                    this.LoadingTextVisibility = Visibility.Visible;

                    this.CanvasViewModel.LoadPixelArt(doc);

                    this.CanvasVisibility = Visibility.Visible;
                    this.LoadingTextVisibility = Visibility.Collapsed;
                });
            }
        }

        private void _save()
        {
            var doc = this.CanvasViewModel.GetPixelArt();
            doc.ApplyMetadata(this.CurrentDocumentMetadata);

            using (var fs = File.OpenWrite(this.DocumentFilePath))
            {
                PixelArtFile.SaveTo(fs, doc);
            }

            this.DocumentChanged = false;
        }

        private void _save_command()
        {
            if (String.IsNullOrEmpty(this.DocumentFilePath) || !File.Exists(this.DocumentFilePath))
            {
                this._saveAs_command();
                return;
            }

            this._save();
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
                this.DocumentFilePath = saveFileDialog.FileName;
                this._save();
            }
        }

        private void _import_command()
        {
            var w = new ImportWindow();
            w.ShowDialog();

            var doc = ((ImportWindowViewModel)w.DataContext).ResultDocument;
            if (doc != null)
            {
                this.CanvasViewModel.LoadPixelArt(doc);

                this.DocumentChanged = true;
                this.CurrentDocumentMetadata = doc;
            }
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

        private void _editDocumentProperty_command()
        {
            var inputDoc = this.CanvasViewModel.GetPixelArt();
            inputDoc.ApplyMetadata(this.CurrentDocumentMetadata);

            var w = new DocumentPropertyWindow();
            var d = (DocumentPropertyWindowViewModel)w.DataContext;

            d.InputDocument = inputDoc;
            w.ShowDialog();

            if (d.OutputDocumentMetadata != null)
            {
                this.DocumentChanged = true;
                this.CurrentDocumentMetadata = d.OutputDocumentMetadata;
            }
        }

        private void _windowClosing(CancelEventArgs e)
        {
            if (this.DocumentChanged)
            {
                var confirm = MessageBox.Show("", this.WindowTitle, MessageBoxButton.YesNoCancel);
                if (confirm == MessageBoxResult.Cancel)
                    e.Cancel = true;
                else if (confirm == MessageBoxResult.Yes)
                    this._save_command();
            }
        }
    }
}
