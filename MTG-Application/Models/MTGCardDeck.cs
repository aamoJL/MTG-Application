using CommunityToolkit.Mvvm.ComponentModel;
using MTGApplication.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using static MTGApplication.Enums;
using static MTGApplication.Services.CommandService;

namespace MTGApplication.Models;

/// <summary>
/// Class for MTG card decks
/// </summary>
public partial class MTGCardDeck : ObservableObject
{
  public class MTGCardDeckCommands
  {
    public class AddCardsToCardlistCommand : ICommand
    {
      private MTGCardDeck CardDeck { get; }
      private CardlistType ListType { get; }
      private MTGCard[] Cards { get; }

      public AddCardsToCardlistCommand(MTGCardDeck cardDeck, CardlistType listType, MTGCard[] cards)
      {
        CardDeck = cardDeck;
        ListType = listType;
        Cards = cards;
      }

      public void Execute()
      {
        if (CardDeck == null)
          return;
        foreach (var item in Cards)
        {
          CardDeck.AddToCardlist(ListType, item);
        }
      }

      public void Undo()
      {
        if (CardDeck == null)
          return;
        var cardlist = CardDeck.GetCardlist(ListType);
        foreach (var item in Cards)
        {
          if (cardlist.FirstOrDefault(x => x.Info.Name == item.Info.Name) is MTGCard existingCard)
          {
            if (existingCard.Count <= item.Count)
            {
              CardDeck.RemoveFromCardlist(ListType, existingCard);
            }
            else
            {
              existingCard.Count -= item.Count;
            }
          }
        }
      }
    }

    public class RemoveCardsFromCardlistCommand : ICommand
    {
      private MTGCardDeck CardDeck { get; }
      private CardlistType ListType { get; }
      private MTGCard[] Cards { get; }

      public RemoveCardsFromCardlistCommand(MTGCardDeck deck, CardlistType listType, MTGCard[] cards)
      {
        CardDeck = deck;
        ListType = listType;
        Cards = cards;
      }

      public void Execute()
      {
        if (CardDeck == null)
          return;
        foreach (var item in Cards)
        {
          CardDeck.RemoveFromCardlist(ListType, item);
        }
      }

      public void Undo()
      {
        if (CardDeck == null)
          return;
        foreach (var item in Cards)
        {
          CardDeck.AddToCardlist(ListType, item);
        }
      }
    }

    public class SetCommanderCommand : ICommand
    {
      private MTGCardDeck CardDeck { get; }
      private MTGCard NewCommander { get; set; }
      private MTGCard OriginalCommander { get; set; }

      public SetCommanderCommand(MTGCardDeck cardDeck, MTGCard commander)
      {
        CardDeck = cardDeck;
        NewCommander = commander;
        OriginalCommander = CardDeck?.Commander;
      }

      public void Execute()
      {
        if(CardDeck != null)
        {
          CardDeck.Commander = NewCommander;
        }
      }

      public void Undo()
      {
        if (CardDeck != null)
        {
          CardDeck.Commander = OriginalCommander;
        }
      }
    }

    public class SetCommanderPartnerCommand : ICommand
    {
      private MTGCardDeck CardDeck { get; }
      private MTGCard NewPartner { get; set; }
      private MTGCard OriginalPartner { get; set; }

      public SetCommanderPartnerCommand(MTGCardDeck cardDeck, MTGCard partner)
      {
        CardDeck = cardDeck;
        NewPartner = partner;
        OriginalPartner = CardDeck?.CommanderPartner;
      }

      public void Execute()
      {
        if (CardDeck != null)
        {
          CardDeck.CommanderPartner = NewPartner;
        }
      }

      public void Undo()
      {
        if (CardDeck != null)
        {
          CardDeck.CommanderPartner = OriginalPartner;
        }
      }
    }
  }

  public enum CombineProperty
  {
    Name, Id
  }

  [ObservableProperty]
  private string name = "";
  [ObservableProperty]
  private MTGCard commander;
  [ObservableProperty]
  private MTGCard commanderPartner;

  public ObservableCollection<MTGCard> DeckCards { get; set; } = new();
  public ObservableCollection<MTGCard> Wishlist { get; set; } = new();
  public ObservableCollection<MTGCard> Maybelist { get; set; } = new();
  public ObservableCollection<MTGCard> Removelist { get; set; } = new();

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
      CardlistType.Removelist => Removelist,
      _ => throw new NotImplementedException(),
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

  /// <summary>
  /// Returns copy of the card deck.
  /// Used for saving the deck to a database
  /// </summary>
  public MTGCardDeck GetCopy()
  {
    return new()
    {
      Name = Name,
      Commander = Commander,
      CommanderPartner = CommanderPartner,
      DeckCards = DeckCards,
      Maybelist = Maybelist,
      Wishlist = Wishlist,
      Removelist = Removelist,
    };
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
    Commander = deck.Commander != null ? new(deck.Commander) : null;
    CommanderPartner = deck.CommanderPartner != null ? new(deck.CommanderPartner) : null;
    DeckCards = deck.DeckCards.Select(x => new MTGCardDTO(x)).ToList();
    WishlistCards = deck.Wishlist.Select(x => new MTGCardDTO(x)).ToList();
    MaybelistCards = deck.Maybelist.Select(x => new MTGCardDTO(x)).ToList();
    RemovelistCards = deck.Removelist.Select(x => new MTGCardDTO(x)).ToList();
  }

  [Key]
  public int Id { get; init; }
  public string Name { get; init; }

  public MTGCardDTO Commander { get; set; }
  public MTGCardDTO CommanderPartner { get; set; }

  [InverseProperty(nameof(MTGCardDTO.DeckCards))]
  public List<MTGCardDTO> DeckCards { get; init; } = new();
  [InverseProperty(nameof(MTGCardDTO.DeckWishlist))]
  public List<MTGCardDTO> WishlistCards { get; init; } = new();
  [InverseProperty(nameof(MTGCardDTO.DeckMaybelist))]
  public List<MTGCardDTO> MaybelistCards { get; init; } = new();
  [InverseProperty(nameof(MTGCardDTO.DeckRemovelist))]
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
