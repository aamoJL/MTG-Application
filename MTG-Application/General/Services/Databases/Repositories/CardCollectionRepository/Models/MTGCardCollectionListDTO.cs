using MTGApplication.Features.CardCollection;
using MTGApplication.General.Services.Databases.Repositories.CardRepository.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MTGApplication.General.Services.Databases.Repositories.CardCollectionRepository.Models;

/// <summary>
/// Data transfer object for <see cref="MTGCardCollectionList"/> class
/// </summary>
public class MTGCardCollectionListDTO
{
  private MTGCardCollectionListDTO() { }
  public MTGCardCollectionListDTO(string name, string searchQuery, List<MTGCardDTO> cards)
  {
    Name = name;
    SearchQuery = searchQuery;
    Cards = cards;
  }

  [Key] public int Id { get; init; }
  public string Name { get; set; }
  public string SearchQuery { get; set; }

  public List<MTGCardDTO> Cards { get; init; } = [];
}
