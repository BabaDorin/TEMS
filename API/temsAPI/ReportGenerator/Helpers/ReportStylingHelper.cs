using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Drawing;

namespace ReportGenerator.Helpers
{
    class ReportStylingHelper
    {

        public double MeasureTextHeight(string text, ExcelFont font, double width)
        {
            if (String.IsNullOrEmpty(text)) return 0;
            
            using (Bitmap bitmap = new Bitmap(1,1))
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                var pixelWidth = Convert.ToInt32(width * 7);  //7 pixels per excel column width
                var fontSize = font.Size * 1.01f;
                var drawingFont = new Font(font.Name, fontSize);
                var size = graphics.MeasureString(
                    text, 
                    drawingFont, 
                    pixelWidth, 
                    new StringFormat { FormatFlags = StringFormatFlags.MeasureTrailingSpaces });

                //72 DPI and 96 points per inch.  Excel height in points with max of 409 per Excel requirements.
                return Math.Min(Convert.ToDouble(size.Height) * 72 / 96, 409);
            }
        }

        public void MergeRowWithWrap(ExcelWorksheet ws, int row, int startCol, int endCol)
        {
            ws.Cells[row, startCol, row, endCol].Merge = true;
            ws.Cells[row, 1].Style.WrapText = true;
        }
    }
}
