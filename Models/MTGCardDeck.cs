using CommunityToolkit.Mvvm.ComponentModel;
using MTGApplication.Interfaces;
using MTGApplication.Models.Converters;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using static MTGApplication.Enums;

namespace MTGApplication.Models
{
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
      ObservableCollection<MTGCard> collection = GetCardlist(listType);
      if (collection == null) { return; }

      if(combineProperty == CombineProperty.Name && collection.FirstOrDefault(x => x.Info.Name == card.Info.Name) is MTGCard existingNameCard)
      {
        existingNameCard.Count += card.Count;
      }
      else if(combineProperty == CombineProperty.Id && collection.FirstOrDefault(x => x.Info.ScryfallId == card.Info.ScryfallId) is MTGCard existingIdCard)
      {
        existingIdCard.Count += card.Count;
      }
      else
      {
        collection.Add(card);
      }
    }
    public void RemoveFromCardlist(CardlistType listType, MTGCard card)
    {
      ObservableCollection<MTGCard> collection = GetCardlist(listType);
      if (collection == null) { return; }

      collection.Remove(card);
    }
  }

  public class MTGCardDeckDTO
  {
    private MTGCardDeckDTO() { }
    public MTGCardDeckDTO(MTGCardDeck deck)
    {
      Name = deck.Name;
      DeckCards.AddRange(deck.DeckCards.Select(x => new CardDTO(x)));
      WishlistCards.AddRange(deck.Wishlist.Select(x => new CardDTO(x)));
      MaybelistCards.AddRange(deck.Maybelist.Select(x => new CardDTO(x)));
    }

    [Key]
    public int Id { get; init; }
    public string Name { get; init; }

    [InverseProperty(nameof(CardDTO.DeckCards))]
    public List<CardDTO> DeckCards { get; init; } = new();

    [InverseProperty(nameof(CardDTO.DeckWishlist))]
    public List<CardDTO> WishlistCards { get; init; } = new();

    [InverseProperty(nameof(CardDTO.DeckMaybelist))]
    public List<CardDTO> MaybelistCards { get; init; } = new();

    public async Task<MTGCardDeck> AsMTGCardDeck(ICardAPI<MTGCard> api)
    {
      return await MTGCardDeckDTOConverter.Convert(this, api);
    }
  }
}
