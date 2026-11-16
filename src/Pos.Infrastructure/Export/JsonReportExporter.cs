using System.Text.Encodings.Web;
using System.Text.Json;
using Pos.Application.Dtos;

namespace Pos.Infrastructure.Export;

public sealed class JsonReportExporter : ReportExporterBase
{
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    public override string Format => "json";

    protected override string Render(IReadOnlyList<FinalGradeDto> rows) =>
        JsonSerializer.Serialize(rows.Select(r => new { student = r.StudentName, grade = r.Grade }), Options);
}
