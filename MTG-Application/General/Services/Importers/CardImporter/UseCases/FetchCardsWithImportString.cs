using MTGApplication.General.ViewModels;
using System;
using System.Threading.Tasks;

namespace MTGApplication.General.Services.Importers.CardImporter.UseCases;

public class FetchCardsWithImportString(IMTGCardImporter importer) : UseCase<string, Task<CardImportResult>>
{
  /// <exception cref="Exception"></exception>
  public override async Task<CardImportResult> Execute(string importString)
    => await importer.ImportWithString(importString);
}
