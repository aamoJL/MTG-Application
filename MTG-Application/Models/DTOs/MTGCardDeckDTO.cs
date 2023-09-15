using MTGApplication.Interfaces;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MTGApplication.Models.DTOs;

/// <summary>
/// Data transfer object for <see cref="MTGCardDeck"/> class
/// </summary>
public class MTGCardDeckDTO
{
  private MTGCardDeckDTO() { }
  public MTGCardDeckDTO(MTGCardDeck deck)
  {
    Name = deck.Name;
    Commander = deck.Commander != null ? new(deck.Commander) : null;
    CommanderPartner = deck.CommanderPartner != null ? new(deck.CommanderPartner) : null;
    DeckCards = deck.DeckCards.Select(x => new MTGCardDTO(x)).ToList();
    WishlistCards = deck.Wishlist.Select(x => new MTGCardDTO(x)).ToList();
    MaybelistCards = deck.Maybelist.Select(x => new MTGCardDTO(x)).ToList();
    RemovelistCards = deck.Removelist.Select(x => new MTGCardDTO(x)).ToList();
  }

  [Key] public int Id { get; init; }
  public string Name { get; init; }
  public MTGCardDTO Commander { get; set; }
  public MTGCardDTO CommanderPartner { get; set; }
  public List<MTGCardDTO> DeckCards { get; init; } = new();
  public List<MTGCardDTO> WishlistCards { get; init; } = new();
  public List<MTGCardDTO> MaybelistCards { get; init; } = new();
  public List<MTGCardDTO> RemovelistCards { get; init; } = new();

  /// <summary>
  /// Converts the DTO to a <see cref="MTGCardDeck"/> object using the <paramref name="api"/>
  /// </summary>
  public async Task<MTGCardDeck> AsMTGCardDeck(ICardAPI<MTGCard> api)
  {
    return new MTGCardDeck()
    {
      Name = Name,
      Commander = Commander != null ? (await api.FetchFromDTOs(new CardDTO[] { Commander })).Found.FirstOrDefault() : null,
      CommanderPartner = CommanderPartner != null ? (await api.FetchFromDTOs(new CardDTO[] { CommanderPartner })).Found.FirstOrDefault() : null,
      DeckCards = new((await api.FetchFromDTOs(DeckCards.ToArray())).Found),
      Wishlist = new((await api.FetchFromDTOs(WishlistCards.ToArray())).Found),
      Maybelist = new((await api.FetchFromDTOs(MaybelistCards.ToArray())).Found),
      Removelist = new((await api.FetchFromDTOs(RemovelistCards.ToArray())).Found),
    };
  }
}
