using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

using Prism.Commands;
using Prism.Mvvm;

using MCPArtNavi.Common;
using MCPArtNavi.UserApp.DocumentPropertyWindowInternal;
using System.ComponentModel;

namespace MCPArtNavi.UserApp
{
    public class DocumentPropertyWindowViewModel : BindableBase
    {
        // 公開プロパティ

        public PixelArtDocumentMetadata OutputDocumentMetadata
        {
            get;
            private set;
        }


        // バインディング プロパティ


        private PixelArtDocument _inputDocument;
        public PixelArtDocument InputDocument
        {
            get => this._inputDocument;
            set => this.SetProperty(ref this._inputDocument, value);
        }

        private string _documentTitle;
        public string DocumentTitle
        {
            get => this._documentTitle;
            set => this.SetProperty(ref this._documentTitle, value);
        }


        private string _documentAuthor;
        public string DocumentAuthor
        {
            get => this._documentAuthor;
            set => this.SetProperty(ref this._documentAuthor, value);
        }

        private string _documentDescription;
        public string DocumentDescription
        {
            get => this._documentDescription;
            set => this.SetProperty(ref this._documentDescription, value);
        }


        private ObservableCollection<UsedMCItem> _usedMCItems;

        public ObservableCollection<UsedMCItem> UsedMCItems
        {
            get => this._usedMCItems;
            set => this.SetProperty(ref this._usedMCItems, value);
        }


        // コマンド

        public ICommand CancelCommand
        {
            get => new DelegateCommand<Object>(this._cancel_command);
        }

        public ICommand OkCommand
        {
            get => new DelegateCommand<Object>(this._ok_command);
        }


        // コンストラクタ

        public DocumentPropertyWindowViewModel()
        {
            this.InputDocument = null;
            this.OutputDocumentMetadata = null;
            this.UsedMCItems = new ObservableCollection<UsedMCItem>();
        }

        
        // 非公開メソッド

        private void _closeWindow(Object e)
        {
            ((Window)e).Close();
        }

        private void _cancel_command(Object e)
        {
            this._closeWindow(e);
        }

        private void _ok_command(Object e)
        {
            this.OutputDocumentMetadata = new PixelArtDocumentMetadata()
            {
                DocumentTitle = this.DocumentTitle,
                DocumentAuthor = this.DocumentAuthor,
                DocumentDescription = this.DocumentDescription,
            };

            this._closeWindow(e);
        }


        // 限定公開メソッド

        protected override void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            // 基底呼び出し
            base.OnPropertyChanged(args);

            // 処理
            if (args.PropertyName == nameof(this.InputDocument))
            {
                this.DocumentTitle = this.InputDocument.DocumentTitle;
                this.DocumentAuthor = this.InputDocument.DocumentAuthor;
                this.DocumentDescription = this.InputDocument.DocumentDescription;

                this.UsedMCItems.Clear();
                foreach (var item in this.InputDocument.Pixels)
                {
                    if (this.UsedMCItems.Any(e => e.Item == item) == false)
                        this.UsedMCItems.Add(new UsedMCItem() { Item = item });
                    this.UsedMCItems.Single(e => e.Item == item).Count++;
                }
            }
        }
    }
}
