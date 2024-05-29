using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace MTGApplication.General.Models.CardCollection;

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

  public List<MTGCardCollectionListDTO> CollectionLists { get; init; } = [];
}