﻿using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Linq;
using static MTGApplication.Services.CommandService;

namespace MTGApplication.Models;

/// <summary>
/// Class for MTG card decks
/// </summary>
public partial class MTGCardDeck : ObservableObject
{
  #region Properties
  [ObservableProperty] private string name = "";
  [ObservableProperty] private MTGCard commander;
  [ObservableProperty] private MTGCard commanderPartner;

  public ObservableCollection<MTGCard> DeckCards { get; set; } = new();
  public ObservableCollection<MTGCard> Wishlist { get; set; } = new();
  public ObservableCollection<MTGCard> Maybelist { get; set; } = new();
  public ObservableCollection<MTGCard> Removelist { get; set; } = new();
  #endregion

  /// <summary>
  /// Returns copy of the card deck.
  /// </summary>
  public MTGCardDeck GetCopy()
  {
    return new()
    {
      Name = Name,
      Commander = Commander != null ? new(Commander.Info) : null,
      CommanderPartner = CommanderPartner != null ? new(CommanderPartner.Info) : null,
      DeckCards = new(DeckCards.Select(x => new MTGCard(x.Info, x.Count))),
      Maybelist = new(Maybelist.Select(x => new MTGCard(x.Info, x.Count))),
      Wishlist = new(Wishlist.Select(x => new MTGCard(x.Info, x.Count))),
      Removelist = new(Removelist.Select(x => new MTGCard(x.Info, x.Count))),
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
          cardlist.Remove(existingCard);
        }
        else
        {
          existingCard.Count -= item.Count;
        }
      }
    }
  }
}

public partial class MTGCardDeck
{
  /// <summary>
  /// <see cref="Services.CommandService"/> commands for <see cref="MTGCardDeck"/>
  /// </summary>
  public class MTGCardDeckCommands
  {
    /// <summary>
    /// Adds cards to a card collection
    /// </summary>
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

    /// <summary>
    /// Removes card from a card collection
    /// </summary>
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

    /// <summary>
    /// Sets deck's commander
    /// </summary>
    public class SetCommanderCommand : ICommand
    {
      private MTGCardDeck CardDeck { get; }
      private MTGCard NewCommander { get; }
      private MTGCard OriginalCommander { get; }

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

    /// <summary>
    /// Sets deck's commander partner
    /// </summary>
    public class SetCommanderPartnerCommand : ICommand
    {
      private MTGCardDeck CardDeck { get; }
      private MTGCard NewPartner { get; }
      private MTGCard OriginalPartner { get; }

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
}