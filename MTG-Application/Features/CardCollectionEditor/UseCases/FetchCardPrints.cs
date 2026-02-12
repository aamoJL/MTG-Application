using MTGApplication.General.Models;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplication.General.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTGApplication.Features.CardCollectionEditor.UseCases;

public class FetchCardPrints(IMTGCardImporter importer) : UseCaseFunc<string, Task<IEnumerable<MTGCard>>>
{
  public IMTGCardImporter Importer { get; } = importer;

  public override async Task<IEnumerable<MTGCard>> Execute(string uri)
    => (await Importer.ImportWithUri(pageUri: uri, paperOnly: true, fetchAll: true)).Found
        .Select(x => new MTGCard(x.Info));
}