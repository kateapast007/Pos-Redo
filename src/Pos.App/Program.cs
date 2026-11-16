using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Pos.App;

Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;
DemoRunner.Run();
using var provider = CompositionRoot.Build();
try
{
    provider.GetRequiredService<ConsoleUI>().Run();
}
catch (EndOfStreamException)
{
    Console.WriteLine("\nВвод завершён.");
}
