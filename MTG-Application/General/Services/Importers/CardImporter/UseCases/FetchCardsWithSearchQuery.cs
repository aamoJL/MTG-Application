using MTGApplication.General.Models;
using MTGApplication.General.ViewModels;
using System.Threading.Tasks;

namespace MTGApplication.General.Services.Importers.CardImporter.UseCases;
public class FetchCardsWithSearchQuery(MTGCardImporter importer) : UseCase<string, Task<CardImportResult<MTGCardInfo>>>
{
  public async override Task<CardImportResult<MTGCardInfo>> Execute(string query)
    => await importer.ImportCardsWithSearchQuery(query);
}