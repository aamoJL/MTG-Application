using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplication.General.ViewModels;
using System.Threading;
using System.Threading.Tasks;

namespace MTGApplication.Features.CardSearch.UseCases;

public class FetchCards(IMTGCardImporter importer) : UseCaseFunc<string, CancellationToken, Task<CardImportResult>>
{
  public IMTGCardImporter Importer { get; private set; } = importer;

  public override async Task<CardImportResult> Execute(string query, CancellationToken token)
    => await Importer.ImportCardsWithSearchQuery(query); // TODO: cancellation token
}