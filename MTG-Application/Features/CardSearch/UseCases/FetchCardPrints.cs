using MTGApplication.General.Models;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplication.General.Services.Importers.CardImporter.UseCases;
using MTGApplication.General.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTGApplication.Features.CardSearch.UseCases;

public class FetchCardPrints(IMTGCardImporter importer) : UseCaseFunc<string, Task<IEnumerable<MTGCard>>>
{
  public override async Task<IEnumerable<MTGCard>> Execute(string uri)
    => (await new FetchCardsWithUri(importer) { FetchAll = true }.Execute(uri)).Found.Select(x => new MTGCard(x.Info));
}
