using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Linq;
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

  [ObservableProperty] private string name = "";
  [ObservableProperty] private MTGCard commander;
  [ObservableProperty] private MTGCard commanderPartner;

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