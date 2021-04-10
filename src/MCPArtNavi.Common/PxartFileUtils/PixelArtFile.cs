using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SimpleProps;

using MCPArtNavi.Common.Items;

namespace MCPArtNavi.Common.PxartFileUtils
{
    public static class PixelArtFile
    {
        private static byte[] _fileMn;

        static PixelArtFile()
        {
            _fileMn = Encoding.ASCII.GetBytes("MCPIXDOC ");
        }

        public static void SaveTo(Stream outputStream, PixelArtDocument document)
        {
            // パレットの生成
            var itemPalette = new List<IMCItem>();
            foreach (var item in document.Pixels)
            {
                if (itemPalette.Contains(item) == false)
                    itemPalette.Add(item);
            }

            // ピクセル データのインデックス変換
            var pixelValues = document.Pixels.Select(item => (short)itemPalette.IndexOf(item)).ToArray();

            // 書き出し
            using (var binaryWriter = new BinaryWriter(outputStream))
            {
                binaryWriter.Write(_fileMn);
                using (var propWriter = new PropWriter(outputStream))
                {
                    var metaSec = new PropSection("mcpixart-file-meta");
                    metaSec.Items.Add(new PropItem("magic-number", PropType.String, "  MCPIXART  "));
                    metaSec.Items.Add(new PropItem("file-description", PropType.String, " This is MC Pixel Art Navi Document File. Please visit our web site: https://www.a32kita.net/ "));
                    metaSec.Items.Add(new PropItem("file-version", PropType.Int64, 1L));
                    metaSec.Items.Add(new PropItem("created-at", PropType.DateTime, DateTime.Now));

                    var docSec = new PropSection("mcpixart-file-doc");
                    docSec.Items.Add(new PropItem("document-title", PropType.String, document.DocumentTitle));
                    docSec.Items.Add(new PropItem("document-author", PropType.String, document.DocumentAuthor));
                    docSec.Items.Add(new PropItem("document-description", PropType.String, document.DocumentDescription));
                    docSec.Items.Add(new PropItem("art-size", PropType.Int16, (short)document.Size));
                    docSec.Items.Add(new PropItem("art-palette", PropType.StringArray, itemPalette.Select(item => item.ItemId).ToArray()));
                    docSec.Items.Add(new PropItem("art-pixels", PropType.Int16Array, pixelValues));

                    propWriter.Write(new Props(new PropSection[] { metaSec, docSec }));
                }
            }
        }

        public static PixelArtDocument LoadFrom(Stream inputStream)
        {
            // ファイル マジック ナンバー
            using (var br = new BinaryReader(inputStream, Encoding.UTF8, true))
            {
                var mnBuf = br.ReadBytes(_fileMn.Length);
                for (var i = 0; i < mnBuf.Length; i++)
                {
                    if (_fileMn[i] != mnBuf[i])
                        throw new Exception("File is not a valid document.");
                }
            }
            
            // 読み取り
            Props props = null;
            using (var propReader = new PropReader(inputStream))
                props = propReader.ReadAllProps();

            // メタチェック
            var invalidFile = false;
            if (((String)props.Sections["mcpixart-file-meta"].Items["magic-number"].Value) != "  MCPIXART  ")
                invalidFile = true;

            if (((Int64)props.Sections["mcpixart-file-meta"].Items["file-version"].Value) != 1)
                invalidFile = true;

            if (invalidFile)
                throw new Exception("File is not a valid document.");

            // ドキュメント ロード
            var documentTitle = (String)props.Sections["mcpixart-file-doc"].Items["document-title"].Value;
            var documentAuthor = (String)props.Sections["mcpixart-file-doc"].Items["document-author"].Value;
            var documentDescription = (String)props.Sections["mcpixart-file-doc"].Items["document-description"].Value;
            var artSize = (PixelArtSize)props.Sections["mcpixart-file-doc"].Items["art-size"].Value;
            var itemPalette = ((String[])props.Sections["mcpixart-file-doc"].Items["art-palette"].Value).Select(itemId => MCItemUtils.GetItemById(itemId)).ToArray();
            var pixels = ((Int16[])props.Sections["mcpixart-file-doc"].Items["art-pixels"].Value).Select(i => itemPalette[i]).ToArray();

            return new PixelArtDocument()
            {
                DocumentTitle = documentTitle,
                DocumentAuthor = documentAuthor,
                DocumentDescription = documentDescription,
                Size = artSize,
                Pixels = pixels,
            };
        }
    }
}
