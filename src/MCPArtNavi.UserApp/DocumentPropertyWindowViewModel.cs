using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        private ObservableCollection<UsedMCItem> _usedMCItems;

        public ObservableCollection<UsedMCItem> UsedMCItems
        {
            get => this._usedMCItems;
            set => this.SetProperty(ref this._usedMCItems, value);
        }


        // コンストラクタ

        public DocumentPropertyWindowViewModel()
        {
            this.InputDocument = null;
            this.OutputDocumentMetadata = null;
            this.UsedMCItems = new ObservableCollection<UsedMCItem>();
        }


        // 限定公開メソッド

        protected override void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            // 基底呼び出し
            base.OnPropertyChanged(args);

            // 処理
            if (args.PropertyName == nameof(this.InputDocument))
            {
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
