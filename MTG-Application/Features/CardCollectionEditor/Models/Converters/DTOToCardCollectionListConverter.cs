using MTGApplication.General.Services.Databases.Repositories.CardCollectionRepository.Models;

namespace MTGApplication.Features.CardCollectionEditor.Models.Converters;

public class DTOToCardCollectionListConverter
{
  public MTGCardCollectionList Convert(MTGCardCollectionListDTO dto)
  {
    return new()
    {
      Name = dto.Name,
      SearchQuery = dto.SearchQuery,
      Cards = [.. dto.Cards]
    };
  }
}