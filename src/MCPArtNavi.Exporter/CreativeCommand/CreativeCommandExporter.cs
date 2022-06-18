using MCPArtNavi.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCPArtNavi.Exporter.CreativeCommand
{
    public class CreativeCommandExporter : ExporterBase
    {
        // 公開プロパティ

        public override ExportOption Option
        {
            get;
            set;
        }


        // 非公開メソッド

        private async Task _writeFillCommand(StreamWriter sw, int startX, int startZ, int endX, int endZ, string itemId)
        {
            await sw.WriteLineAsync($"fill ~{startX} ~0 ~{startZ} ~{endX} ~0 ~{endZ} {itemId}");
        }


        // 公開メソッド

        /// <summary>
        /// 
        /// </summary>
        /// <param name="document">マップアート データ</param>
        /// <param name="stream">このオプションは使用されません。</param>
        /// <param name="baseDirectory">出力先ディレクトリ</param>
        /// <returns></returns>
        public override async Task<ExportResult> ExportAsync(PixelArtDocument document, Stream stream, string baseDirectory)
        {
            if (baseDirectory == null)
                throw new ArgumentNullException(nameof(baseDirectory));
            
            if (File.Exists(baseDirectory) || Directory.Exists(baseDirectory))
            {
                throw new ArgumentException(nameof(baseDirectory));
            }

            var baseName = Path.GetFileName(baseDirectory);
            var utf8nEnc = new UTF8Encoding(false);

            Directory.CreateDirectory(baseDirectory);

            using (var swPackMeta = new StreamWriter(Path.Combine(baseDirectory, "pack.mcmeta"), false, utf8nEnc))
            {
                swPackMeta.WriteLine("{");
                swPackMeta.WriteLine("  \"pack\": {");
                swPackMeta.WriteLine("    \"pack_format\": 1,");
                swPackMeta.WriteLine("    \"description\": \"datapack\"");
                swPackMeta.WriteLine("  }");
                swPackMeta.WriteLine("}");
            }

            var dataDirPath = Path.Combine(baseDirectory, "data");
            Directory.CreateDirectory(dataDirPath);

            var packDirPath = Path.Combine(dataDirPath, baseName);
            Directory.CreateDirectory(packDirPath);

            var functionsDir = Path.Combine(packDirPath, "functions");
            Directory.CreateDirectory(functionsDir);

            var mcFunctionFilePath = Path.Combine(functionsDir, baseName + ".mcfunction");

            var sw = new StreamWriter(mcFunctionFilePath, false, utf8nEnc);
            IMCItem currentItem = document.Pixels[0];
            var currentItemStartPositionX = 0;
            var currentItemStartPositionZ = 0;

            for (var row = 0; row < document.Size.GetHeight(); row++)
            {
                for (var col = 0; col < document.Size.GetWidth(); col++)
                {
                    var nextItemNum = (row * document.Size.GetWidth()) + col;
                    var nextItem = document.Pixels[nextItemNum];
                    if (currentItem != nextItem)
                    {
                        if (col == 0)
                        {
                            // 左端の場合 → 先に変更
                            currentItem = nextItem;
                        }

                        // コマンドの出力
                        await this._writeFillCommand(sw, currentItemStartPositionX, currentItemStartPositionZ, col, row, currentItem.ItemId);

                        // currentItem の変更
                        currentItem = nextItem;
                        currentItemStartPositionX = col + 1;
                    }
                }

                // コマンドの出力
                await this._writeFillCommand(sw, currentItemStartPositionX, currentItemStartPositionZ, document.Size.GetWidth(), row, currentItem.ItemId);
                currentItemStartPositionX = 0;
                currentItemStartPositionZ = row + 1;
            }

            await sw.FlushAsync();
            return new ExportResult();
        }
    }
}
