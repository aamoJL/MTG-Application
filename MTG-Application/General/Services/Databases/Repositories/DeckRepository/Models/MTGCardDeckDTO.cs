using MTGApplication.Features.DeckEditor.Models;
using MTGApplication.General.Services.Databases.Repositories.CardRepository.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MTGApplication.General.Services.Databases.Repositories.DeckRepository.Models;

/// <summary>
/// Data transfer object for <see cref="MTGCardDeck"/> class
/// </summary>
public record MTGCardDeckDTO
{
  private MTGCardDeckDTO() { }
  public MTGCardDeckDTO(string name, MTGCardDTO commander = null, MTGCardDTO partner = null, List<MTGCardDTO> deckCards = null, List<MTGCardDTO> wishlistCards = null, List<MTGCardDTO> maybelistCards = null, List<MTGCardDTO> removelistCards = null)
  {
    Name = name;
    Commander = commander;
    CommanderPartner = partner;
    DeckCards = deckCards ?? [];
    WishlistCards = wishlistCards ?? [];
    MaybelistCards = maybelistCards ?? [];
    RemovelistCards = removelistCards ?? [];
  }

  [Key] public int Id { get; set; }
  public string Name { get; set; }
  public MTGCardDTO Commander { get; set; }
  public MTGCardDTO CommanderPartner { get; set; }
  public List<MTGCardDTO> DeckCards { get; set; } = [];
  public List<MTGCardDTO> WishlistCards { get; set; } = [];
  public List<MTGCardDTO> MaybelistCards { get; set; } = [];
  public List<MTGCardDTO> RemovelistCards { get; set; } = [];
}