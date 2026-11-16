using Microsoft.Extensions.DependencyInjection;
using Pos.Application;
using Pos.Application.Ports;
using Pos.Application.UseCases;
using Pos.Domain.Evaluations;
using Pos.Domain.Grading;
using Pos.Domain.Rubrics;
using Pos.Domain.Students;
using Pos.Infrastructure.Export;
using Pos.Infrastructure.Persistence;

namespace Pos.App;

public static class CompositionRoot
{
    public static ServiceProvider Build()
    {
        var services = new ServiceCollection();
        services.AddSingleton<EvaluationFactory>();
        services.AddSingleton<FinalGradeCalculator>();
        services.AddSingleton<IStudentRepository, InMemoryStudentRepository>();
        services.AddSingleton<IRubricRepository, InMemoryRubricRepository>();
        services.AddSingleton<IEvaluationRepository, InMemoryEvaluationRepository>();
        services.AddSingleton<IReportExporter, CsvReportExporter>();
        services.AddSingleton<IReportExporter, JsonReportExporter>();
        services.AddSingleton<StudentUseCases>();
        services.AddSingleton<RubricUseCases>();
        services.AddSingleton<EvaluationUseCases>();
        services.AddSingleton<ReportUseCases>();
        services.AddSingleton<PosFacade>();
        services.AddSingleton<ConsoleUI>();
        return services.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });
    }
}
