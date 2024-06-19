using MTGApplication.Features.CardCollectionEditor.CardCollectionList.Models;
using MTGApplication.General.Services.Databases.Repositories.CardCollectionRepository.Models;
using MTGApplication.General.Services.Databases.Repositories.CardRepository.Models;
using System.Linq;

namespace MTGApplication.Features.CardCollectionEditor.CardCollection.Services.Converters;

public class CollectionListToDTOConverter
{
  public static MTGCardCollectionListDTO Convert(MTGCardCollectionList list)
  {
    return new MTGCardCollectionListDTO(
      name: list.Name,
      searchQuery: list.SearchQuery,
      cards: list.Cards.Select(x => new MTGCardDTO(x.Info)).ToList());
  }
}