using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplication.General.Services.Importers.CardImporter.UseCases;
using MTGApplication.General.ViewModels;
using System.Threading.Tasks;

namespace MTGApplication.Features.CardCollectionEditor.UseCases;

public class FetchCardsWithImportText(IMTGCardImporter importer) : UseCaseFunc<string, Task<CardImportResult>>
{
  public override async Task<CardImportResult> Execute(string importText)
    => await new FetchCardsWithImportString(importer).Execute(importText);
}