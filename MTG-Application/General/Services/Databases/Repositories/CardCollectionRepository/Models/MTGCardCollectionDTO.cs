using MTGApplication.Features.CardCollectionEditor.CardCollection.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MTGApplication.General.Services.Databases.Repositories.CardCollectionRepository.Models;

/// <summary>
/// Data transfer object for <see cref="MTGCardCollection"/> class
/// </summary>
public record MTGCardCollectionDTO
{
  private MTGCardCollectionDTO() { }
  public MTGCardCollectionDTO(string name, List<MTGCardCollectionListDTO> collectionLists)
  {
    Name = name;
    CollectionLists = collectionLists;
  }

  [Key] public int Id { get; init; }
  [Required] public string Name { get; init; } = string.Empty;

  public List<MTGCardCollectionListDTO> CollectionLists { get; init; } = [];
}