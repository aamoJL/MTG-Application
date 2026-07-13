using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplication.General.Services.Importers.CardImporter.UseCases;
using MTGApplication.General.ViewModels;
using System.Threading;
using System.Threading.Tasks;

namespace MTGApplication.Features.CardSearch.UseCases;

public class FetchCards(IMTGCardImporter importer) : UseCaseFunc<string, Task<CardImportResult>>
{
  public CancellationToken? CancellationToken { get; init; } = null;

  public override async Task<CardImportResult> Execute(string query)
    => await new FetchCardsWithSearchQuery(importer)
    {
      CancellationToken = CancellationToken
    }.Execute(query);
}