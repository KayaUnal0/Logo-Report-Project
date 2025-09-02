using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Text;

// KULLANIM DIŞI
namespace Infrastructure.Logic.Templates
{
    public static class HtmlTableBuilder
    {
        private const string HeaderBg = "#4A90E2";
        private const string HeaderFg = "#FFFFFF";
        private const string OddRowBg = "#FFFFFF";
        private const string EvenRowBg = "#F4F6F8";
        private const string BorderClr = "#DDDDDD";
        private const string FontFamily = "Arial, Helvetica, sans-serif";

        public static string FromTsvLines(List<string> tsvLines)
        {
            if (tsvLines == null || tsvLines.Count == 0)
                return "<em>Sonuç bulunamadı</em>";

            var sb = new StringBuilder();

            sb.Append("<div style=\"max-width:100%;overflow:auto;\">");

            sb.Append("<table cellpadding=\"6\" cellspacing=\"0\" ")
              .Append("style=\"border-collapse:collapse;width:100%;font-size:13px;")
              .Append("font-family:").Append(FontFamily).Append(";color:#333;\">");

            //  HEADER 
            var headers = tsvLines[0].Split('\t');
            sb.Append("<thead><tr style=\"background:").Append(HeaderBg)
              .Append(";color:").Append(HeaderFg).Append(";\">");

            foreach (var h in headers)
            {
                sb.Append("<th style=\"border:1px solid ").Append(BorderClr)
                  .Append(";font-weight:bold;text-align:left;\">")
                  .Append(WebUtility.HtmlEncode(h))
                  .Append("</th>");
            }
            sb.Append("</tr></thead>");

            //  BODY 
            sb.Append("<tbody>");
            for (int i = 1; i < tsvLines.Count; i++)
            {
                var rowBg = (i % 2 == 0) ? EvenRowBg : OddRowBg;
                sb.Append("<tr style=\"background:").Append(rowBg).Append(";\">");

                var cells = tsvLines[i].Split('\t');
                foreach (var c in cells)
                {
                    var cell = c ?? string.Empty;
                    var align = IsNumeric(cell) ? "right" : "left";

                    sb.Append("<td style=\"border:1px solid ").Append(BorderClr)
                      .Append(";white-space:nowrap;text-align:").Append(align).Append(";\">")
                      .Append(WebUtility.HtmlEncode(cell))
                      .Append("</td>");
                }

                sb.Append("</tr>");
            }
            sb.Append("</tbody></table></div>");

            return sb.ToString();
        }
        private static bool IsNumeric(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return false;
            return double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out _)
                || double.TryParse(value, NumberStyles.Any, CultureInfo.CurrentCulture, out _);
        }
    }
}
