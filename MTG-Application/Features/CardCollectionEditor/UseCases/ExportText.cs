using MTGApplication.General.Services.Exporters;
using MTGApplication.General.ViewModels;

namespace MTGApplication.Features.CardCollectionEditor.UseCases;

public class ExportText(IExporter<string> exporter) : UseCaseAction<string>
{
  public override void Execute(string text) => exporter.Export(text);
}