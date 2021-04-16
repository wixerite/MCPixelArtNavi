using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Microsoft Corporation Open XML SDK
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

        static SpreadSheetExporter()
        {
            _columnChars = new char[]
            {
                'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'
            };
        }

        //private Cell _insertCellInWorkSheet(string columnName, uint rowIndex, Worksheet workSheet)
        //{
        //    var sheetData = workSheet.GetFirstChild<SheetData>();
        //    var cellReference = columnName + rowIndex.ToString();

        //    var row = sheetData.Elements<Row>().FirstOrDefault(r => r.RowIndex == rowIndex);
        //    if (row == null)
        //    {
        //        row = new Row() { RowIndex = rowIndex };
        //    }
        //}

        private string _leftToColumnName(int left)
        {
            if (left < 0)
                return String.Empty;

            return this._leftToColumnName(left / _columnChars.Length - 1) + _columnChars[left % 26];
        }

        private uint _topToRowNumber(int top)
        {
            return (uint)(top + 1);
        }

        private string _toCellReference(int left, int top)
        {
            return this._leftToColumnName(left) + this._topToRowNumber(top).ToString();
        }

        public override async Task<ExportResult> ExportAsync(PixelArtDocument document, Stream stream)
        {
            var spreadsheetDocument = SpreadsheetDocument.Create(stream, SpreadsheetDocumentType.Workbook);
            
            var workbookPart = spreadsheetDocument.AddWorkbookPart();
            workbookPart.Workbook = new Workbook();

            var worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
            worksheetPart.Worksheet = new Worksheet(
                // SheetData よりも先に Columns が必要
                new Columns(new Column()
                {
                    Min = 1u,
                    Max = (uint)document.Size.GetWidth(),
                    Width = 3.3,
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
            var fillList = enabledItems.Select(e =>
            {
                // FFRRGGBB
                var backgroundColorCode = "FF" + e.ItemColor.Replace("#", "").ToUpper();

                var pFill = new PatternFill() { PatternType = PatternValues.Solid };
                pFill.Append(new ForegroundColor() { Rgb = HexBinaryValue.FromString(backgroundColorCode) });
                pFill.Append(new BackgroundColor() { Rgb = HexBinaryValue.FromString(backgroundColorCode) });

                var fill = new Fill();
                fill.Append(pFill);

                return fill;
            }).ToList();

            // デフォルト fill の追加 (頭に追加してデフォルト化)
            fillList.Insert(0, new Fill()
            {
                PatternFill = new PatternFill() { PatternType = PatternValues.None }
            });

            // 予約枠を潰す
            fillList.Insert(1, new Fill()
            {
                PatternFill = new PatternFill() { PatternType = PatternValues.Gray125 }
            });

            // デフォルト fill を含む、すべての fill に対応する cellFormat の生成
            var cellFormatList = fillList.Select((e, idx) =>
            {
                return new CellFormat()
                {
                    FontId = 0,
                    FillId = (uint)idx, // fill と同じインデックス番号
                    BorderId = 0,
                };
            }).ToList();

            stylesPart.Stylesheet = new Stylesheet(new Fonts(new Font(new Color() { Theme = UInt32Value.FromUInt32(1U) })
            {
                FontName = new FontName() { Val = StringValue.FromString("Arial") },
                FontSize = new FontSize() { Val = DoubleValue.FromDouble(11d) },
                FontFamilyNumbering = new FontFamilyNumbering() { Val = 2 },
                FontCharSet = new FontCharSet() { Val = 128 },
                FontScheme = new FontScheme() { Val = FontSchemeValues.Minor },
            }), new Fills(fillList), new Borders(new Border()), new CellFormats(cellFormatList), new CellStyles(new CellStyle()
            {
                Name = StringValue.FromString("Normal"),
                FormatId = 0,
                BuiltinId = 0,
            }), new DifferentialFormats(), new TableStyles()
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
                        //StyleIndex = 2,
                    };
                    rows[i].InsertAt(cell, j);
                }

                sheetData.Append(rows[i]);
            }

            //var p = 0;
            //for (var i = 0; i < document.Size.GetWidth(); i++)
            //{
            //    for (var j = 0; j < document.Size.GetHeight(); j++)
            //    {
            //        var cell = 
            //    }
            //}

            workbookPart.Workbook.Save();
            spreadsheetDocument.Close();

            return new ExportResult();
        }
    }
}
