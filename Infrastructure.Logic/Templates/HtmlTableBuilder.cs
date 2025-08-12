using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Infrastructure.Logic.Templates
{
    public static class HtmlTableBuilder
    {
        public static string FromTsvLines(List<string> tsvLines)
        {
            if (tsvLines == null || tsvLines.Count == 0)
                return "<em>Sonuç bulunamadı</em>";

            var sb = new StringBuilder();

            // Email-client friendly, inline CSS
            sb.Append(
                "<div style=\"max-width:100%;overflow:auto\">" +
                "<table border=\"1\" cellpadding=\"6\" cellspacing=\"0\" " +
                "style=\"border-collapse:collapse;font-family:Consolas,Monaco,monospace;font-size:12px;white-space:nowrap;\">");

            // Header
            var headers = tsvLines[0].Split('\t');
            sb.Append("<thead><tr>");
            foreach (var h in headers)
                sb.Append("<th style=\"background:#f5f5f5;text-align:left\">")
                  .Append(WebUtility.HtmlEncode(h))
                  .Append("</th>");
            sb.Append("</tr></thead><tbody>");

            // Rows
            for (int i = 1; i < tsvLines.Count; i++)
            {
                sb.Append("<tr>");
                var cells = tsvLines[i].Split('\t');
                foreach (var c in cells)
                    sb.Append("<td>").Append(WebUtility.HtmlEncode(c)).Append("</td>");
                sb.Append("</tr>");
            }

            sb.Append("</tbody></table></div>");
            return sb.ToString();
        }
    }
}
