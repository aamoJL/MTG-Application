using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplication.General.ViewModels;
using System.Threading.Tasks;

namespace MTGApplication.Features.CardCollectionEditor.UseCases;

public class FetchCardsWithQuery(IMTGCardImporter importer) : UseCaseFunc<string, Task<CardImportResult>>
{
  public IMTGCardImporter Importer { get; } = importer;

  public override async Task<CardImportResult> Execute(string query)
    => await Importer.ImportCardsWithSearchQuery(query, pagination: false);
}