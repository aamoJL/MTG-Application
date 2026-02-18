using MTGApplication.General.Services.Exporters;
using MTGApplication.General.ViewModels;
using System.Threading.Tasks;

namespace MTGApplication.Features.DeckEditor.UseCases;

public class ExportText(IExporter<string> exporter) : UseCaseFunc<string, Task>
{
  public IExporter<string> Exporter { get; } = exporter;

  public override async Task Execute(string text) => await Exporter.Export(text);
}