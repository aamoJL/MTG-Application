using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplication.General.ViewModels;
using System.Threading.Tasks;

namespace MTGApplication.Features.CardCollectionEditor.UseCases;

public class FetchCardsWithImportText(IMTGCardImporter importer) : UseCaseFunc<string, Task<CardImportResult>>
{
  public IMTGCardImporter Importer { get; } = importer;

  public override async Task<CardImportResult> Execute(string importText)
    => await Importer.ImportWithString(importText);
}