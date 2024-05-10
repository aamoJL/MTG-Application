using MTGApplication.General.Models.Card;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace MTGApplication.General.Models.CardCollection;

/// <summary>
/// Data transfer object for <see cref="MTGCardCollectionList"/> class
/// </summary>
public class MTGCardCollectionListDTO
{
  private MTGCardCollectionListDTO() { }
  public MTGCardCollectionListDTO(MTGCardCollectionList list)
  {
    Name = list.Name;
    SearchQuery = list.SearchQuery;
    Cards.AddRange(list.Cards.Select(x => new MTGCardDTO(x)));
  }

  [Key] public int Id { get; init; }
  public string Name { get; set; }
  public string SearchQuery { get; set; }

  public List<MTGCardDTO> Cards { get; init; } = new();
}
