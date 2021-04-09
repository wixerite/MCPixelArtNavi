using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MCPArtNavi.Common;
using MCPArtNavi.Common.Items;
using MCPArtNavi.Importer.ImageImporterInternals;

namespace MCPArtNavi.Importer
{
    public class ImageImporter : ImporterBase
    {
        // 非公開フィールド
        private PixelArtSize _targetSize;
        private ImporterPalette _itemPalette;


        // 公開プロパティ

        public override PixelArtSize TargetSize
        {
            get => this._targetSize;
        }

        public override ImporterPalette ItemPalette
        {
            get => this._itemPalette;
        }


        // コンストラクタ

        public ImageImporter()
        {
            this._targetSize = PixelArtSize.Size128x128;
            
            this._itemPalette = new ImporterPalette();
            this._itemPalette.Items.AddRange(MCItemUtils.EnabledItems);
        }


        // 非公開メソッド

        private IMCItem _getNearlyColorItem(Color color)
        {
            // アイテムと色の距離を辞書化
            var diffDict = new Dictionary<IMCItem, double>();
            foreach (var item in this.ItemPalette.Items)
            {
                diffDict[item] = XyzDifference.Difference(color, item.GetColor());
            }

            return diffDict.OrderBy(kvp => kvp.Value).First().Key;
        }


        // 公開メソッド

        public async override Task<PixelArtDocument> ImportAsync(Stream stream, string documentTitle)
        {
            var targetWidth = this._targetSize.GetWidth();
            var targetHeight = this._targetSize.GetHeight();
            var pixels = new IMCItem[targetWidth * targetHeight];

            try
            {
                await Task.Run(async () =>
                {
                    // 非同期セクション

                    using (var image = Image.FromStream(stream))
                    using (var bitmap = new Bitmap(image))
                    {
                        var mapToImageMagnWidth = (double)bitmap.Width / targetWidth;
                        var mapToImageMagnHeight = (double)bitmap.Height / targetHeight;

                        var p = 0;
                        for (var i = 0; i < targetHeight; i++)
                        {
                            for (var j = 0; j < targetWidth; j++, p++)
                            {
                                var color = bitmap.GetPixel(
                                    Math.Min((int)(j * mapToImageMagnWidth), bitmap.Width),
                                    Math.Min((int)(i * mapToImageMagnHeight), bitmap.Height));
                                pixels[p] = this._getNearlyColorItem(color);
                            }
                        }
                    }
                });
            }
            catch(Exception ex)
            {
                throw ex;
            }

            return new PixelArtDocument()
            {
                DocumentTitle = documentTitle,
                DocumentAuthor = "",
                DocumentDescription = "",
                Size = this._targetSize,
                Pixels = pixels
            };
        }

        public void SetTargetSize(PixelArtSize size)
        {
            this._targetSize = size;
        }
    }
}
