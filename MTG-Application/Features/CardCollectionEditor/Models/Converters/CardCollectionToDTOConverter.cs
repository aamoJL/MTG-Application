using MTGApplication.General.Services.Databases.Repositories.CardCollectionRepository.Models;
using System.Linq;

namespace MTGApplication.Features.CardCollectionEditor.Models.Converters;

public class CardCollectionToDTOConverter
{
  public static MTGCardCollectionDTO Convert(MTGCardCollection collection)
  {
    return new MTGCardCollectionDTO(
      name: collection.Name,
      collectionLists: [.. collection.CollectionLists.Select(CollectionListToDTOConverter.Convert)]);
  }
}
