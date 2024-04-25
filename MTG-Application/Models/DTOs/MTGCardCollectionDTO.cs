using MTGApplication.API.CardAPI;
using MTGApplication.General.Models.Card;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MTGApplication.Models.DTOs;

/// <summary>
/// Data transfer object for <see cref="MTGCardCollection"/> class
/// </summary>
public class MTGCardCollectionDTO
{
  private MTGCardCollectionDTO() { }
  public MTGCardCollectionDTO(MTGCardCollection collection)
  {
    Name = collection.Name;
    CollectionLists = collection.CollectionLists.Select(x => new MTGCardCollectionListDTO(x)).ToList();
  }

  [Key] public int Id { get; init; }
  public string Name { get; init; }

  public List<MTGCardCollectionListDTO> CollectionLists { get; init; } = new();

  /// <summary>
  /// Converts the DTO to a <see cref="MTGCardCollection"/> object using the given <paramref name="api"/>
  /// </summary>
  public async Task<MTGCardCollection> AsMTGCardCollection(ICardAPI<MTGCard> api)
  {
    return new MTGCardCollection()
    {
      Name = Name,
      CollectionLists = new(await Task.WhenAll(CollectionLists.Select(async x =>
      {
        return new MTGCardCollectionList()
        {
          Name = x.Name,
          SearchQuery = x.SearchQuery,
          Cards = new((await api.FetchFromDTOs(x.Cards.ToArray())).Found),
        };
      })))
    };
  }
}
