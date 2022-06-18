using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Open XML SDK
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

using MCPArtNavi.Common;
using MCPArtNavi.Common.Items;

namespace MCPArtNavi.Exporter.OpenXML
{
    public class SpreadSheetExporter : ExporterBase
    {
        private static char[] _columnChars;

        public override ExportOption Option
        {
            get;
            set;
        }

        /// <summary>
        /// <see cref="SpreadSheetExporter"/> クラスを初期化します。
        /// </summary>
        static SpreadSheetExporter()
        {
            // カラム名に使用するアルファベット
            _columnChars = new char[]
            {
                'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'
            };
        }

        /// <summary>
        /// 0 起点の左端からの距離をもとにアルファベットから成るカラム名を生成します。(例: AB)
        /// </summary>
        /// <param name="left"></param>
        /// <returns></returns>
        private string _leftToColumnName(int left)
        {
            if (left < 0)
                return String.Empty;

            return this._leftToColumnName(left / _columnChars.Length - 1) + _columnChars[left % 26];
        }

        /// <summary>
        /// 0 起点の上端からの距離をもとに数値の行番号を生成します。(例: 21)
        /// </summary>
        /// <param name="top"></param>
        /// <returns></returns>
        private uint _topToRowNumber(int top)
        {
            return (uint)(top + 1);
        }

        /// <summary>
        /// <see cref="Cell"/> の CellReference プロパティ用のセルの位置情報を生成します。 (例: AB21)
        /// </summary>
        /// <param name="left"></param>
        /// <param name="top"></param>
        /// <returns></returns>
        private string _toCellReference(int left, int top)
        {
            return this._leftToColumnName(left) + this._topToRowNumber(top).ToString();
        }

        /// <summary>
        /// <see cref="IMCItem.ItemColor"/> の文字列 (#RRGGBB 形式) から Fill 用のカラーコード (AARRGGBB 形式) を生成します。
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private string _mcItemToArgbCode(IMCItem item)
        {
            return "FF" + item.ItemColor.Replace("#", "").ToUpper();
        }

        /// <summary>
        /// <see cref="IMCItem"/> の色情報を使用して、<see cref="Fill"/> を生成します。
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private Fill _createFillFromMCItem(IMCItem item)
        {
            // "#RRGGBB" => "FFRRGGBB"
            var backgroundColorCode = HexBinaryValue.FromString(this._mcItemToArgbCode(item));

            var pFill = new PatternFill() { PatternType = PatternValues.Solid };
            pFill.Append(new ForegroundColor() { Rgb = backgroundColorCode });
            pFill.Append(new BackgroundColor() { Rgb = backgroundColorCode });

            var fill = new Fill();
            fill.Append(pFill);

            return fill;
        }

        /// <summary>
        /// Fill のインデックス番号を参照する <see cref="CellFormat"/> を生成します。
        /// </summary>
        /// <param name="fillIndex"></param>
        /// <returns></returns>
        private CellFormat _createCellFormatFromFill(int fillIndex)
        {
            return new CellFormat()
            {
                FontId = 0,
                FillId = (uint)fillIndex, // fill と同じインデックス番号
                BorderId = 0,
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="document">マップアート データ</param>
        /// <param name="stream">書き込み先ファイルのストリーム</param>
        /// <param name="baseDirectory">このオプションは使用されません</param>
        /// <returns></returns>
        public override async Task<ExportResult> ExportAsync(PixelArtDocument document, Stream stream, string baseDirectory)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            var spreadsheetDocument = SpreadsheetDocument.Create(stream, SpreadsheetDocumentType.Workbook);
            
            var workbookPart = spreadsheetDocument.AddWorkbookPart();
            workbookPart.Workbook = new Workbook();

            var worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
            worksheetPart.Worksheet = new Worksheet(
                // SheetData よりも先に Columns が必要
                new Columns(new Column()
                {
                    // 列の幅など
                    Min = 1u,
                    Max = (uint)document.Size.GetWidth(),
                    Width = 3,
                    CustomWidth = true,
                }),
                new SheetData());

            var sheets = spreadsheetDocument.WorkbookPart.Workbook.AppendChild<Sheets>(new Sheets());

            var sheet = new Sheet()
            {
                Id = spreadsheetDocument.WorkbookPart.GetIdOfPart(worksheetPart),
                SheetId = 1,
                Name = "MCPixelArtNavi"
            };
            sheets.Append(sheet);

            var sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();
            var stylesPart = workbookPart.AddNewPart<WorkbookStylesPart>();

            // スタイル生成 :: アイテム塗りつぶしの生成
            var enabledItems = MCItemUtils.EnabledItems.ToList();
            var fillList = new List<Fill>();

            // Fill[0]: デフォルト fill の追加 (頭に追加してデフォルト化)
            fillList.Add(new Fill()
            {
                // 固定値
                PatternFill = new PatternFill() { PatternType = PatternValues.None }
            });

            // Fill[1]: 予約枠を潰す
            fillList.Add(new Fill()
            {
                // 固定値
                PatternFill = new PatternFill() { PatternType = PatternValues.Gray125 }
            });

            // Fill[2 ～]: アイテムの色情報の Fill
            fillList.AddRange(enabledItems.Select(this._createFillFromMCItem));

            // デフォルト fill を含む、すべての fill に対応する cellFormat の生成
            var cellFormatList = fillList.Select((e, idx) => this._createCellFormatFromFill(idx)).ToList();

            // デフォルト フォント (特に中身にこだわりはない)
            var font = new Font(new Color() { Theme = UInt32Value.FromUInt32(1U) })
            {
                FontName = new FontName() { Val = StringValue.FromString("Arial") },
                FontSize = new FontSize() { Val = DoubleValue.FromDouble(11d) },
                FontFamilyNumbering = new FontFamilyNumbering() { Val = 2 },
                FontCharSet = new FontCharSet() { Val = 128 },
                FontScheme = new FontScheme() { Val = FontSchemeValues.Minor },
            };

            // デフォルト セル スタイル
            var cellStyle = new CellStyle()
            {
                Name = StringValue.FromString("Normal"),
                FormatId = 0,
                BuiltinId = 0,
            };

            stylesPart.Stylesheet = new Stylesheet(
                new Fonts(font),
                new Fills(fillList),
                new Borders(new Border()),
                new CellFormats(cellFormatList),
                new CellStyles(cellStyle),
                new DifferentialFormats(),
                new TableStyles()
                {
                    DefaultTableStyle = StringValue.FromString("TableStyleMedium9"),
                    DefaultPivotStyle = StringValue.FromString("PivotStyleLight16"),
                });


            // 行作成
            var rows = new Row[document.Size.GetHeight()];
            var p = 0;
            for (var i = 0; i < rows.Length; i++)
            {
                // 列作成
                rows[i] = new Row() { RowIndex = new UInt32Value((uint)(i + 1)) };
                for (var j = 0; j < document.Size.GetWidth(); j++, p++)
                {
                    var cell = new Cell()
                    {
                        CellReference = this._toCellReference(j, i),
                        CellValue = new CellValue(""),
                        DataType = CellValues.String,
                        StyleIndex = (uint)(enabledItems.IndexOf(document.Pixels[p]) + 2),
                    };
                    rows[i].InsertAt(cell, j);
                }

                sheetData.Append(rows[i]);
            }

            workbookPart.Workbook.Save();
            spreadsheetDocument.Close();

            return new ExportResult();
        }
    }
}
