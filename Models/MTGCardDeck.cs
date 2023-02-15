using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using static MTGApplication.Enums;

namespace MTGApplication.Models
{
  public partial class MTGCardDeck : ObservableObject
  {
    [ObservableProperty]
    private string name = "";

    public ObservableCollection<MTGCard> DeckCards { get; set; } = new();
    public ObservableCollection<MTGCard> Wishlist { get; set; } = new();
    public ObservableCollection<MTGCard> Maybelist { get; set; } = new();

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
    public void AddToCardlist(CardlistType listType, MTGCard card)
    {
      ObservableCollection<MTGCard> collection = GetCardlist(listType);
      if(collection == null) { return; }

      if(collection.FirstOrDefault(x => x.Info.ScryfallId == card.Info.ScryfallId) is MTGCard existingCard)
      {
        existingCard.Count += card.Count;
      }
      else
      {
        collection.Add(card);
      }
    }
    public void RemoveFromCardlist(CardlistType listType, MTGCard card)
    {
      ObservableCollection<MTGCard> collection = GetCardlist(listType);
      if(collection == null) { return; }

      collection.Remove(card);
    }
  }

  public class MTGCardDeckDTO
  {
    public MTGCardDeckDTO(MTGCardDeck deck)
    {
      Name = deck.Name;
      DeckCards.AddRange(deck.DeckCards);
      WishlistCards.AddRange(deck.Wishlist);
      MaybelistCards.AddRange(deck.Maybelist);
    }

    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
    public List<MTGCard> DeckCards { get; set; } = new();
    public List<MTGCard> WishlistCards { get; set; } = new();
    public List<MTGCard> MaybelistCards { get; set; } = new();

    public MTGCardDeck AsMTGCardDeck()
    {
      return new()
      {
        Name = Name,
        DeckCards = new ObservableCollection<MTGCard>(DeckCards),
        Wishlist = new ObservableCollection<MTGCard>(WishlistCards),
        Maybelist = new ObservableCollection<MTGCard>(MaybelistCards),
      };
    }
  }
}
