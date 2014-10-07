using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CarlosAg.ExcelXmlWriter;

namespace Mere.Excel
{
    public static class MereExcelExtensions
    {
        public static Workbook ToExcel<T>(this IList<T> src) where T : new()
        {
            var s = (IEnumerable<T>) src;
            return s.ToExcel();
        }

        public static Workbook ToExcel<T>(this IEnumerable<T> src) where T : new()
        {
            var myWb = new Workbook();
            myWb.AddStyles();
            var myWs = myWb.Worksheets.Add("one");

            var headerRow = myWs.Table.Rows.Add();
            var cols = MereUtils.CacheCheck<T>().SelectMereColumns.ToList();

            foreach (var field in cols)
            {
                var myCol = new WorksheetColumn(150);
                myWs.Table.Columns.Add(myCol);
                headerRow.Cells.Add(field.DisplayName, DataType.String, "HeaderStyle");
            }

            foreach (var rec in src)
            {
                var newRow = myWs.Table.Rows.Add();
                newRow.AutoFitHeight = false;
                foreach (var field in cols)
                {
                    var v = field.GetBase(rec);
                    var val = v == null ? "" : v.ToString();
                    newRow.Cells.Add(val, DataType.String, (myWs.Table.Rows.IndexOf(newRow) % 2) == 0 ? "s31" : "s31Odd");
                }
            }

            return myWb;
        }

        public static MemoryStream ToExcelMemoryStream<T>(this IEnumerable<T> src) where T : new()
        {
            var memStrm = new MemoryStream();
            src.ToExcel().Save(memStrm);
            return memStrm;
        }

        public static void ToExcelFile<T>(this IEnumerable<T> src, string filePath) where T : new()
        {
            src.ToExcel().Save(filePath);
        }

        public static MemoryStream ToExcelMemoryStream<T>(this IList<T> src) where T : new()
        {
            var memStrm = new MemoryStream();
            src.ToExcel().Save(memStrm);
            return memStrm;
        }

        public static void ToExcelFile<T>(this IList<T> src, string filePath) where T : new()
        {
            src.ToExcel().Save(filePath);
//            using (var fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write))
//                src.ToExcel().Save(fs);
        }

        public static void AddStyles(this Workbook myWb)
        {
            const string evenBg = "#ffffff";
            const string oddBg = "#dddddd";
            //var thirdBG = "#AABCC1";

            // font
            const string font = "Tahoma";
            const string fontColor = "#000000";
            const int fontSize = 11;

            const string borderColor = "#aaaaaa";

            const string s31NumFormat = "@";
            //var s3100NumFormat = "###,###,##0.00";
            //var s310000NumFormat = "###,###,##0.0000";

            WorksheetStyle style = myWb.Styles.Add("HeaderStyle");
            style.Font.FontName = font;
            style.Font.Size = 11;
            style.Font.Bold = false;
            style.Alignment.Horizontal = StyleHorizontalAlignment.Center;
            style.Font.Color = "White";
            style.Interior.Color = "#333333";
            style.Alignment.WrapText = true;
            style.Borders.Add(StylePosition.Bottom, LineStyleOption.Continuous, 1, borderColor);
            style.Borders.Add(StylePosition.Left, LineStyleOption.Continuous, 1, borderColor);
            style.Borders.Add(StylePosition.Right, LineStyleOption.Continuous, 1, borderColor);
            style.Borders.Add(StylePosition.Top, LineStyleOption.Continuous, 1, borderColor);
            style.Interior.Pattern = StyleInteriorPattern.Solid;
            WorksheetStyle s31 = myWb.Styles.Add("s31");
            s31.Font.FontName = font;
            s31.Font.Size = fontSize;
            s31.Font.Color = fontColor;
            s31.Alignment.Horizontal = StyleHorizontalAlignment.Center;
            s31.Alignment.Vertical = StyleVerticalAlignment.Center;
            s31.Interior.Color = evenBg;
            s31.Interior.Pattern = StyleInteriorPattern.Solid;
            s31.Alignment.WrapText = false;
            s31.Borders.Add(StylePosition.Bottom, LineStyleOption.Continuous, 1, borderColor);
            s31.Borders.Add(StylePosition.Left, LineStyleOption.Continuous, 1, borderColor);
            s31.Borders.Add(StylePosition.Right, LineStyleOption.Continuous, 1, borderColor);
            s31.Borders.Add(StylePosition.Top, LineStyleOption.Continuous, 1, borderColor);
            s31.NumberFormat = s31NumFormat;

            WorksheetStyle s31Odd = myWb.Styles.Add("s31Odd");
            s31Odd.Font.FontName = font;
            s31Odd.Font.Size = fontSize;
            s31Odd.Font.Color = fontColor;
            s31Odd.Alignment.Horizontal = StyleHorizontalAlignment.Center;
            s31Odd.Alignment.Vertical = StyleVerticalAlignment.Center;
            s31Odd.Interior.Color = oddBg;
            s31Odd.Interior.Pattern = StyleInteriorPattern.Solid;
            s31Odd.Alignment.WrapText = false;
            s31Odd.Borders.Add(StylePosition.Bottom, LineStyleOption.Continuous, 1, borderColor);
            s31Odd.Borders.Add(StylePosition.Left, LineStyleOption.Continuous, 1, borderColor);
            s31Odd.Borders.Add(StylePosition.Right, LineStyleOption.Continuous, 1, borderColor);
            s31Odd.Borders.Add(StylePosition.Top, LineStyleOption.Continuous, 1, borderColor);
            s31Odd.NumberFormat = s31NumFormat;
        }
    }
}
