using MTGApplication.General.Services.Exporters;
using MTGApplication.General.ViewModels;

namespace MTGApplication.Features.DeckEditor.UseCases;

public class ExportText(IExporter<string> exporter) : UseCaseAction<string>
{
  public IExporter<string> Exporter { get; } = exporter;

  public override void Execute(string text) => Exporter.Export(text);
}