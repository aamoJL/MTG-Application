using Microsoft.Extensions.Caching.Memory;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplication.General.Services.Importers.CardImporter.UseCases;
using MTGApplication.General.ViewModels;
using System.Threading.Tasks;

namespace MTGApplication.Features.CardCollectionEditor.UseCases;

public class FetchCardsWithQuery(IMTGCardImporter importer) : UseCaseFunc<string, Task<CardImportResult>>
{
  public bool Pagination { get; init; } = false;
  public IMemoryCache? Cache { get; init; } = null;

  public override async Task<CardImportResult> Execute(string query)
    => await new FetchCardsWithSearchQuery(importer) { FetchAll = !Pagination, Cache = Cache }.Execute(query);
}