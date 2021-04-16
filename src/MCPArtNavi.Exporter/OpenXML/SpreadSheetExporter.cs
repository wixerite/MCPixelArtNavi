using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            worksheetPart.Worksheet = new Worksheet(new SheetData());

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

            // スタイル生成
            var enabledItems = MCItemUtils.EnabledItems;
            var fills = new Fills(enabledItems.Select(e =>
            {
                var pFill = new PatternFill() { PatternType = PatternValues.Solid };
                pFill.Append(new BackgroundColor() { Rgb = e.ItemColor.Replace("#", "") });

                var fill = new Fill();
                fill.Append(pFill);

                return (Fill)fill;
            }).ToArray());

            stylesPart.Stylesheet = new Stylesheet(fills, new Fonts(new Font()
            {
                FontName = new FontName() { Val = StringValue.FromString("Arial") },
                FontSize = new FontSize() { Val = DoubleValue.FromDouble(10.5) },
            }));

            var borders = new Borders();
            var border = new Border();
            borders.Append(border);


            // 行作成
            var rows = new Row[document.Size.GetHeight()];
            var p = 0;
            for (var i = 0; i < rows.Length; i++)
            {
                rows[i] = new Row() { RowIndex = new UInt32Value((uint)(i + 1)) };
                for (var j = 0; j < document.Size.GetWidth(); j++, p++)
                {
                    var cell = new Cell()
                    {
                        CellReference = this._toCellReference(j, i),
                        CellValue = new CellValue("*"),
                        DataType = CellValues.String,
                        //StyleIndex = enabledItems. document.Pixels[p]
                        StyleIndex = 1
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
