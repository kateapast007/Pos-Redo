using System.Globalization;
using System.Text;
using Pos.Application.Dtos;

namespace Pos.Infrastructure.Export;

public sealed class CsvReportExporter : ReportExporterBase
{
    public override string Format => "csv";

    protected override string Render(IReadOnlyList<FinalGradeDto> rows)
    {
        var builder = new StringBuilder("Студент;Итог\n");
        foreach (var row in rows)
            builder.Append(Escape(row.StudentName)).Append(';').Append(row.Grade.ToString("0.##", CultureInfo.InvariantCulture)).Append('\n');
        return builder.ToString();
    }

    private static string Escape(string value) =>
        value.Contains(';') || value.Contains('"') ? $"\"{value.Replace("\"", "\"\"")}\"" : value;
}
