using CommunityToolkit.Mvvm.ComponentModel;
using MTGApplication.Interfaces;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using static MTGApplication.Enums;

namespace MTGApplication.Models;

/// <summary>
/// Class for MTG card decks
/// </summary>
public partial class MTGCardDeck : ObservableObject
{
  public enum CombineProperty
  {
    Name, Id
  }

  [ObservableProperty]
  private string name = "";

  public ObservableCollection<MTGCard> DeckCards { get; set; } = new();
  public ObservableCollection<MTGCard> Wishlist { get; set; } = new();
  public ObservableCollection<MTGCard> Maybelist { get; set; } = new();

  /// <summary>
  /// Returns the cardlist that is associated with the given <paramref name="listType"/>
  /// </summary>
  public ObservableCollection<MTGCard> GetCardlist(CardlistType listType)
  {
    return listType switch
    {
      CardlistType.Deck => DeckCards,
      CardlistType.Wishlist => Wishlist,
      CardlistType.Maybelist => Maybelist,
      _ => null,
    };
  }

  /// <summary>
  /// Adds card to given <paramref name="listType"/>.
  /// Card will be combined to a card with the same name, if <paramref name="combineName"/> is <see langword="true"/>, otherwise
  /// the card will be combined to a card with the same ID.
  /// </summary>
  public void AddToCardlist(CardlistType listType, MTGCard card, CombineProperty combineProperty = CombineProperty.Name)
  {
    var collection = GetCardlist(listType);
    if (collection == null)
    { return; }

    if (combineProperty == CombineProperty.Name && collection.FirstOrDefault(x => x.Info.Name == card.Info.Name) is MTGCard existingNameCard)
    {
      existingNameCard.Count += card.Count;
    }
    else if (combineProperty == CombineProperty.Id && collection.FirstOrDefault(x => x.Info.ScryfallId == card.Info.ScryfallId) is MTGCard existingIdCard)
    {
      existingIdCard.Count += card.Count;
    }
    else
    {
      collection.Add(card);
    }
  }

  /// <summary>
  /// Removes <paramref name="card"/> from the cardlist that is assiciated with the <paramref name="listType"/>
  /// </summary>
  public void RemoveFromCardlist(CardlistType listType, MTGCard card)
  {
    var collection = GetCardlist(listType);
    if (collection == null)
    { return; }
    collection.Remove(card);
  }
}

/// <summary>
/// Data transfer object for <see cref="MTGCardDeck"/> class
/// </summary>
public class MTGCardDeckDTO
{
  private MTGCardDeckDTO() { }
  public MTGCardDeckDTO(MTGCardDeck deck)
  {
    Name = deck.Name;
    DeckCards = deck.DeckCards.Select(x => new MTGCardDTO(x)).ToList();
    WishlistCards = deck.Wishlist.Select(x => new MTGCardDTO(x)).ToList();
    MaybelistCards = deck.Maybelist.Select(x => new MTGCardDTO(x)).ToList();
  }

  [Key]
  public int Id { get; init; }
  public string Name { get; init; }

  [InverseProperty(nameof(MTGCardDTO.DeckCards))]
  public List<MTGCardDTO> DeckCards { get; init; } = new();
  [InverseProperty(nameof(MTGCardDTO.DeckWishlist))]
  public List<MTGCardDTO> WishlistCards { get; init; } = new();
  [InverseProperty(nameof(MTGCardDTO.DeckMaybelist))]
  public List<MTGCardDTO> MaybelistCards { get; init; } = new();

  /// <summary>
  /// Converts the DTO to a <see cref="MTGCardDeck"/> object using the <paramref name="api"/>
  /// </summary>
  public async Task<MTGCardDeck> AsMTGCardDeck(ICardAPI<MTGCard> api)
  {
    return new MTGCardDeck()
    {
      Name = Name,
      DeckCards = new((await api.FetchFromDTOs(DeckCards.ToArray())).Found),
      Wishlist = new((await api.FetchFromDTOs(WishlistCards.ToArray())).Found),
      Maybelist = new((await api.FetchFromDTOs(MaybelistCards.ToArray())).Found),
    };
  }
}
