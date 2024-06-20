using MTGApplication.General.ViewModels;
using System.Threading.Tasks;

namespace MTGApplication.General.Services.Importers.CardImporter.UseCases;

public class FetchCardsWithImportString(MTGCardImporter importer) : UseCase<string, Task<CardImportResult>>
{
  public override async Task<CardImportResult> Execute(string importString)
    => await importer.ImportFromString(importString);
}