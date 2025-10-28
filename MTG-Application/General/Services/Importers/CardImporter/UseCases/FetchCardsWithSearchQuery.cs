using MTGApplication.General.ViewModels;
using System.Threading.Tasks;

namespace MTGApplication.General.Services.Importers.CardImporter.UseCases;

public class FetchCardsWithSearchQuery(IMTGCardImporter importer) : UseCase<string, Task<CardImportResult>>
{
  /// <exception cref="InvalidOperationException"></exception>
  /// <exception cref="System.Net.Http.HttpRequestException"></exception>
  /// <exception cref="UriFormatException"></exception>
  /// <exception cref="System.Text.Json.JsonException"></exception>
  public async override Task<CardImportResult> Execute(string query)
    => await importer.ImportCardsWithSearchQuery(query);
}