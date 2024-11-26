using MTGApplication.General.ViewModels;
using System;
using System.Threading.Tasks;

namespace MTGApplication.General.Services.Importers.CardImporter.UseCases;

public class FetchCardsWithImportString(MTGCardImporter importer) : UseCase<string, Task<CardImportResult>>
{
  /// <exception cref="InvalidOperationException"></exception>
  /// <exception cref="System.Net.Http.HttpRequestException"></exception>
  /// <exception cref="UriFormatException"></exception>
  public override async Task<CardImportResult> Execute(string importString)
  {
    try
    {
      return await importer.ImportFromString(importString);
    }
    catch { throw; }
  }
}