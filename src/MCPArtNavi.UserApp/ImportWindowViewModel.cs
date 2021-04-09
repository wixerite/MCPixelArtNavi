using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using Microsoft.Win32;

using Prism.Commands;
using Prism.Mvvm;

using MCPArtNavi.Common;
using MCPArtNavi.Common.Items;
using MCPArtNavi.Importer;
using MCPArtNavi.UserApp.ImportWindowInternal;

namespace MCPArtNavi.UserApp
{
    public class ImportWindowViewModel : BindableBase
    {
        // 公開プロパティ

        public PixelArtDocument ResultDocument
        {
            get;
            private set;
        }


        // バインディング プロパティ

        private ObservableCollection<ImporterMCItem> _mCItems;

        public ObservableCollection<ImporterMCItem> MCItems
        {
            get => this._mCItems;
            set => this.SetProperty(ref this._mCItems, value);
        }

        private PixelArtSizeItem[] _artSizes;

        public PixelArtSizeItem[] ArtSizes
        {
            get => this._artSizes;
            set => this.SetProperty(ref this._artSizes, value);
        }


        private PixelArtSizeItem _importSize;

        public PixelArtSizeItem ImportSize
        {
            get => this._importSize;
            set => this.SetProperty(ref this._importSize, value);
        }


        private string _importFilePath;

        public string ImportFilePath
        {
            get => this._importFilePath;
            set => this.SetProperty(ref this._importFilePath, value);
        }


        // コマンド

        public DelegateCommand ReferenceFileCommand
        {
            get => new DelegateCommand(this._referenceFile_command);
        }

        public DelegateCommand<Object> CancelCommand
        {
            get => new DelegateCommand<Object>(this._cancel_command);
        }

        public DelegateCommand<Object> ImportCommand
        {
            get => new DelegateCommand<Object>(this._import_command);
        }



        // コンストラクタ

        public ImportWindowViewModel()
        {
            this.MCItems = new ObservableCollection<ImporterMCItem>(
                MCItemUtils.EnabledItems.Select(e => new ImporterMCItem() { Item = e, Use = true }));

            this.ArtSizes = Enum.GetValues(typeof(PixelArtSize)).Cast<PixelArtSize>().Select(e => new PixelArtSizeItem(e)).ToArray();

            //this.DragEventHandler = (sender, e) =>
            //{
            //    var dropFiles = e.Data.GetData(DataFormats.FileDrop) as string[];
            //    if (dropFiles == null || dropFiles.Length == 0)
            //        return;

            //    ImportFilePath = dropFiles[0];
            //};

            this.ResultDocument = null;
        }


        // 非公開メソッド

        private void _referenceFile_command()
        {
            var openFileDialog = new OpenFileDialog()
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Filter = "Image File (*.png;*.bmp)|*.png;*.bmp|All Files (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
                this.ImportFilePath = openFileDialog.FileName;
        }

        private void _cancel_command(Object e)
        {
            this._closeWindow(e);
        }

        private void _import_command(Object e)
        {
            using (var fs = File.OpenRead(this.ImportFilePath))
            {
                var importer = new ImageImporter();
                importer.SetTargetSize(this.ImportSize.Value);
                importer.ItemPalette.Items.Clear();
                importer.ItemPalette.Items.AddRange(this.MCItems.Where(elem => elem.Use).Select(elem => elem.Item));

                this.ResultDocument = Task.Run(async () => await importer.ImportAsync(fs, "Imported Art")).Result;
            }

            this._closeWindow(e);
        }

        private void _closeWindow(Object e)
        {
            ((Window)e).Close();
        }
    }
}
