using CommunityToolkit.Mvvm.ComponentModel;
using MTGApplication.General.Models.Card;
using System.Collections.ObjectModel;
using System.Linq;

namespace MTGApplication.General.Models.CardDeck;

/// <summary>
/// Class for MTG card decks
/// </summary>
public partial class MTGCardDeck : ObservableObject
{
  [ObservableProperty] private string name = "";
  [ObservableProperty] private MTGCard commander;
  [ObservableProperty] private MTGCard commanderPartner;

  public ObservableCollection<MTGCard> DeckCards { get; set; } = new();
  public ObservableCollection<MTGCard> Wishlist { get; set; } = new();
  public ObservableCollection<MTGCard> Maybelist { get; set; } = new();
  public ObservableCollection<MTGCard> Removelist { get; set; } = new();

  public int DeckSize => DeckCards.Sum(x => x.Count) + (Commander != null ? 1 : 0) + (CommanderPartner != null ? 1 : 0);
}

//public partial class MTGCardDeck
//{
//  /// <summary>
//  /// <see cref="Services.CommandService"/> commands for <see cref="MTGCardDeck"/>
//  /// </summary>
//  public class MTGCardDeckCommands
//  {
//    /// <summary>
//    /// Adds cards to a card collection
//    /// </summary>
//    public class AddCardsToCardlistCommand : ICommand
//    {
//      ObservableCollection<MTGCard> Cardlist { get; }
//      private MTGCard[] Cards { get; }

//      public AddCardsToCardlistCommand(ObservableCollection<MTGCard> cardlist, MTGCard[] cards)
//      {
//        Cardlist = cardlist;
//        Cards = cards;
//      }

//      public void Execute()
//      {
//        if (Cardlist == null) return;
//        AddOrCombineToCardlist(Cards, Cardlist);
//      }

//      public void Undo()
//      {
//        if (Cardlist == null) return;
//        RemoveOrReduceFromCardlist(Cards, Cardlist);
//      }
//    }

//    /// <summary>
//    /// Removes card from a card collection
//    /// </summary>
//    public class RemoveCardsFromCardlistCommand : ICommand
//    {
//      private ObservableCollection<MTGCard> Cardlist { get; }
//      private MTGCard[] Cards { get; }

//      public RemoveCardsFromCardlistCommand(ObservableCollection<MTGCard> cardlist, MTGCard[] cards)
//      {
//        Cardlist = cardlist;
//        Cards = cards;
//      }

//      public void Execute()
//      {
//        if (Cardlist == null) return;
//        RemoveOrReduceFromCardlist(Cards, Cardlist);
//      }

//      public void Undo()
//      {
//        if (Cardlist == null) return;
//        AddOrCombineToCardlist(Cards, Cardlist);
//      }
//    }

//    /// <summary>
//    /// Sets deck's commander
//    /// </summary>
//    public class SetCommanderCommand : ICommand
//    {
//      private MTGCardDeck CardDeck { get; }
//      private MTGCard NewCommander { get; }
//      private MTGCard OriginalCommander { get; }

//      public SetCommanderCommand(MTGCardDeck cardDeck, MTGCard commander)
//      {
//        CardDeck = cardDeck;
//        NewCommander = commander;
//        OriginalCommander = CardDeck?.Commander;
//      }

//      public void Execute()
//      {
//        if (CardDeck != null)
//        {
//          CardDeck.Commander = NewCommander;
//        }
//      }

//      public void Undo()
//      {
//        if (CardDeck != null)
//        {
//          CardDeck.Commander = OriginalCommander;
//        }
//      }
//    }

//    /// <summary>
//    /// Sets deck's commander partner
//    /// </summary>
//    public class SetCommanderPartnerCommand : ICommand
//    {
//      private MTGCardDeck CardDeck { get; }
//      private MTGCard NewPartner { get; }
//      private MTGCard OriginalPartner { get; }

//      public SetCommanderPartnerCommand(MTGCardDeck cardDeck, MTGCard partner)
//      {
//        CardDeck = cardDeck;
//        NewPartner = partner;
//        OriginalPartner = CardDeck?.CommanderPartner;
//      }

//      public void Execute()
//      {
//        if (CardDeck != null)
//        {
//          CardDeck.CommanderPartner = NewPartner;
//        }
//      }

//      public void Undo()
//      {
//        if (CardDeck != null)
//        {
//          CardDeck.CommanderPartner = OriginalPartner;
//        }
//      }
//    }
//  }
//}