using CommunityToolkit.Mvvm.ComponentModel;
using MTGApplication.Interfaces;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
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
      ObservableCollection<MTGCard> Cardlist { get; }
      private MTGCard[] Cards { get; }

      public AddCardsToCardlistCommand(ObservableCollection<MTGCard> cardlist, MTGCard[] cards)
      {
        Cardlist = cardlist;
        Cards = cards;
      }

      public void Execute()
      {
        if (Cardlist == null) return;
        AddOrCombineToCardlist(Cards, Cardlist);
      }

      public void Undo()
      {
        if (Cardlist == null) return;
        RemoveOrReduceFromCardlist(Cards, Cardlist);
      }
    }

    public class RemoveCardsFromCardlistCommand : ICommand
    {
      private ObservableCollection<MTGCard> Cardlist { get; }
      private MTGCard[] Cards { get; }

      public RemoveCardsFromCardlistCommand(ObservableCollection<MTGCard> cardlist, MTGCard[] cards)
      {
        Cardlist = cardlist;
        Cards = cards;
      }

      public void Execute()
      {
        if (Cardlist == null) return;
        RemoveOrReduceFromCardlist(Cards, Cardlist);
      }

      public void Undo()
      {
        if (Cardlist == null) return;
        AddOrCombineToCardlist(Cards, Cardlist);
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
        if (CardDeck != null)
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

  /// <summary>
  /// For each card the array, the card will be added to the cardlist if a card with the same name does not already exist in the list, otherwise the card's count will be added to the existing card's count.
  /// </summary>
  public static void AddOrCombineToCardlist(MTGCard[] cards, ObservableCollection<MTGCard> cardlist)
  {
    foreach (var card in cards)
    {
      if (cardlist.FirstOrDefault(x => x.Info.Name == card.Info.Name) is MTGCard existingNameCard)
      {
        existingNameCard.Count += card.Count;
      }
      else
      {
        cardlist.Add(card);
      }
    }
  }

  /// <summary>
  /// For each card the array, the card will be removed from the cardlist if a card with the same name does not already exist in the list, otherwise the card's count will be reduced from the existing card's count.
  /// </summary>
  public static void RemoveOrReduceFromCardlist(MTGCard[] cards, ObservableCollection<MTGCard> cardlist)
  {
    foreach (var item in cards)
    {
      if (cardlist.FirstOrDefault(x => x.Info.Name == item.Info.Name) is MTGCard existingCard)
      {
        if (existingCard.Count <= item.Count)
        {
          cardlist.Remove(item);
        }
        else
        {
          existingCard.Count -= item.Count;
        }
      }
    }
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
