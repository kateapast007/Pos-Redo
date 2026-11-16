using Xunit;
using Pos.Application.Dtos;
using Pos.Infrastructure.Export;

namespace Pos.Tests;

public sealed class ReportExporterTests
{
    private static readonly FinalGradeDto[] Grades =
    [
        new(2, "Петрова Мария", 7.5m),
        new(1, "Иванов Иван", 8.6m)
    ];

    [Fact]
    public void Csv_WritesHeaderAndRowsSortedByName_AndAddsExtension()
    {
        var written = new CsvReportExporter().Export(Grades, Path.Combine(Path.GetTempPath(), $"pos-test-{Guid.NewGuid():N}"));
        try
        {
            Assert.EndsWith(".csv", written);
            var lines = File.ReadAllLines(written);
            Assert.StartsWith("Студент;Итог", lines[0]);
            Assert.Equal("Иванов Иван;8.6", lines[1]);
            Assert.Equal("Петрова Мария;7.5", lines[2]);
        }
        finally
        {
            File.Delete(written);
        }
    }

    [Fact]
    public void Json_WritesCyrillicAsIs()
    {
        var written = new JsonReportExporter().Export(Grades, Path.Combine(Path.GetTempPath(), $"pos-test-{Guid.NewGuid():N}"));
        try
        {
            Assert.EndsWith(".json", written);
            Assert.Contains("Иванов Иван", File.ReadAllText(written));
        }
        finally
        {
            File.Delete(written);
        }
    }
}
