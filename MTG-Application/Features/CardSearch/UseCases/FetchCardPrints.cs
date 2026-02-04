using MTGApplication.General.Models;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplication.General.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTGApplication.Features.CardSearch.UseCases;

public class FetchCardPrints(IMTGCardImporter importer) : UseCaseFunc<MTGCardInfo, Task<IEnumerable<MTGCard>>>
{
  public IMTGCardImporter Importer { get; private set; } = importer;

  public override async Task<IEnumerable<MTGCard>> Execute(MTGCardInfo info)
  {
    return (await Importer.ImportWithUri(pageUri: info.PrintSearchUri, paperOnly: true, fetchAll: true))
        .Found.Select(x => new MTGCard(x.Info));
  }
}
