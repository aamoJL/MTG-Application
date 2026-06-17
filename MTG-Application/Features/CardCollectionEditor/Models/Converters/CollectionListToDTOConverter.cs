using MTGApplication.General.Services.Databases.Repositories.CardCollectionRepository.Models;

namespace MTGApplication.Features.CardCollectionEditor.Models.Converters;

public class CollectionListToDTOConverter
{
  public static MTGCardCollectionListDTO Convert(MTGCardCollectionList list)
  {
    return new MTGCardCollectionListDTO(
      name: list.Name,
      searchQuery: list.SearchQuery,
      cards: [.. list.Cards]);
  }
}