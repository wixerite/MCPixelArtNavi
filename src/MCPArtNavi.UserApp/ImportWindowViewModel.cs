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

        private string _cannotImportReason;

        public string CannotImportReason
        {
            get => this._cannotImportReason;
            set => this.SetProperty(ref this._cannotImportReason, value);
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
                MCItemUtils.EnabledItems
                    .Select(e => new ImporterMCItem() { Item = e, Use = e is MCWoolTypeBase }));

            this.ArtSizes = Enum.GetValues(typeof(PixelArtSize)).Cast<PixelArtSize>().Select(e => new PixelArtSizeItem(e)).ToArray();
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
            if (!this._import_command_canExecute(null))
            {
                MessageBox.Show(this.CannotImportReason);
                return;
            }

            try
            {
                using (var fs = File.OpenRead(this.ImportFilePath))
                {
                    var importer = new ImageImporter();
                    importer.SetTargetSize(this.ImportSize.Value);
                    importer.ItemPalette.Items.Clear();
                    importer.ItemPalette.Items.AddRange(this.MCItems.Where(elem => elem.Use).Select(elem => elem.Item));

                    Exception ex = null;

                    var doc = Task.Run(async () => await importer.ImportAsync(fs, Path.GetFileNameWithoutExtension(this.ImportFilePath)).ContinueWith(r =>
                    {
                        ex = r.Exception;
                        if (ex is AggregateException)
                            ex = ((AggregateException)ex).InnerException;

                        if (ex == null)
                            return r.Result;
                        else
                            return null;
                    })).Result;

                    if (ex != null)
                        throw ex;

                    this.ResultDocument = doc;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failure to import image;\n{ex.GetType().Name}: {ex.Message}");
                return;
            }

            this._closeWindow(e);
        }

        private bool _import_command_canExecute(Object e)
        {
            if (this.MCItems.Where(elem => elem.Use).Count() < 2)
            {
                this.CannotImportReason = "You must select 2 or more wool or other items on checking list.";
                return false;
            }

            if (String.IsNullOrEmpty(this.ImportFilePath) || !File.Exists(this.ImportFilePath))
            {
                this.CannotImportReason = "You must select exist image file.";
                return false;
            }

            this.CannotImportReason = "";

            return true;
        }

        private void _closeWindow(Object e)
        {
            ((Window)e).Close();
        }
    }
}
