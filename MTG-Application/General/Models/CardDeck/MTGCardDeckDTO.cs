using MTGApplication.General.Models.Card;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;

namespace MTGApplication.General.Models.CardDeck;

/// <summary>
/// Data transfer object for <see cref="MTGCardDeck"/> class
/// </summary>
public record MTGCardDeckDTO
{
  public static Expression<Func<MTGCardDeckDTO, object>>[] DefaultIncludes => new Expression<Func<MTGCardDeckDTO, object>>[]
  {
    x => x.DeckCards,
    x => x.WishlistCards,
    x => x.MaybelistCards,
    x => x.RemovelistCards,
    x => x.Commander,
    x => x.CommanderPartner,
  };

  private MTGCardDeckDTO() { }

  public MTGCardDeckDTO(string name, MTGCardDTO commander = null, MTGCardDTO partner = null, List<MTGCardDTO> deckCards = null, List<MTGCardDTO> wishlistCards = null, List<MTGCardDTO> maybelistCards = null, List<MTGCardDTO> removelistCards = null)
  {
    Name = name;
    Commander = commander;
    CommanderPartner = partner;
    DeckCards = deckCards ?? new();
    WishlistCards = wishlistCards ?? new();
    MaybelistCards = maybelistCards ?? new();
    RemovelistCards = removelistCards ?? new();
  }

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

  [Key] public int Id { get; set; }
  public string Name { get; set; }
  public MTGCardDTO Commander { get; set; }
  public MTGCardDTO CommanderPartner { get; set; }
  public List<MTGCardDTO> DeckCards { get; set; } = new();
  public List<MTGCardDTO> WishlistCards { get; set; } = new();
  public List<MTGCardDTO> MaybelistCards { get; set; } = new();
  public List<MTGCardDTO> RemovelistCards { get; set; } = new();
}