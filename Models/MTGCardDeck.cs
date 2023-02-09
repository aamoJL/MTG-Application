using CommunityToolkit.Mvvm.ComponentModel;
using MTGApplication.Database;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MTGApplication.Models
{
  
  public partial class MTGCardDeck : ObservableObject
  {
    public enum SortDirection { ASC, DESC }
    public enum SortProperty { CMC, Name, Rarity, Color, Set, Count, Price }
    public enum CardlistType { Deck, Wishlist, Maybelist }

    public MTGCardDeck()
    {
      DeckCards.CollectionChanged += DeckCards_CollectionChanged;
      Wishlist.CollectionChanged += Wishlist_CollectionChanged;
      Maybelist.CollectionChanged += Maybelist_CollectionChanged;
    }

    private void Maybelist_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
      switch (e.Action)
      {
        case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
          MTGCard newCard = e.NewItems[0] as MTGCard;
          newCard.MTGCardDeckMaybelist = this;
          newCard.PropertyChanged += MTGCardDeck_PropertyChanged;
          break;
        case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
          (e.OldItems[0] as MTGCard).PropertyChanged -= MTGCardDeck_PropertyChanged;
          break;
        case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
          break;
        default:
          break;
      }

      OnPropertyChanged(nameof(MaybelistSize));
    }
    private void Wishlist_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
      switch (e.Action)
      {
        case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
          MTGCard newCard = e.NewItems[0] as MTGCard;
          newCard.MTGCardDeckWishlist = this;
          newCard.PropertyChanged += MTGCardDeck_PropertyChanged;
          break;
        case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
          (e.OldItems[0] as MTGCard).PropertyChanged -= MTGCardDeck_PropertyChanged;
          break;
        case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
          break;
        default:
          break;
      }

      OnPropertyChanged(nameof(WishlistSize));
    }
    private void DeckCards_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
      switch (e.Action)
      {
        case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
          MTGCard newCard = e.NewItems[0] as MTGCard;
          newCard.MTGCardDeckDeckCards = this;
          newCard.PropertyChanged += MTGCardDeck_PropertyChanged;
          break;
        case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
          (e.OldItems[0] as MTGCard).PropertyChanged -= MTGCardDeck_PropertyChanged;
          break;
        case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
          break;
        default:
          break;
      }

      OnPropertyChanged(nameof(DeckSize));
    }
    private void MTGCardDeck_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      if (e.PropertyName == nameof(MTGCard.Count))
      {
        OnPropertyChanged(nameof(DeckSize));
        OnPropertyChanged(nameof(WishlistSize));
        OnPropertyChanged(nameof(MaybelistSize));
      }
    }

    [ObservableProperty]
    private string name = "";

    [Key]
    public int MTGCardDeckId { get; set; }

    [InverseProperty(nameof(MTGCard.MTGCardDeckDeckCards))]
    public ObservableCollection<MTGCard> DeckCards { get; set; } = new();
    [InverseProperty(nameof(MTGCard.MTGCardDeckWishlist))]
    public ObservableCollection<MTGCard> Wishlist { get; set; } = new();
    [InverseProperty(nameof(MTGCard.MTGCardDeckMaybelist))]
    public ObservableCollection<MTGCard> Maybelist { get; set; } = new();

    public int DeckSize => DeckCards.Sum(x => x.Count);
    public int WishlistSize => Wishlist.Sum(x => x.Count);
    public int MaybelistSize => Maybelist.Sum(x => x.Count);

    public void AddToCardlist(CardlistType listType, MTGCard card)
    {
      ObservableCollection<MTGCard> collection = null;

      switch (listType)
      {
        case CardlistType.Deck: collection = DeckCards; break;
        case CardlistType.Wishlist: collection = Wishlist; break;
        case CardlistType.Maybelist: collection = Maybelist; break;
        default: return;
      }

      if(collection.FirstOrDefault(x => x.ScryfallId == card.ScryfallId) is MTGCard existingCard)
      {
        existingCard.Count += card.Count;
      }
      else
      {
        collection.Add(card);
      }
    }

    public static bool Exists(string name)
    {
      using var db = new CardDatabaseContext();
      return db.MTGCardDecks.SingleOrDefault(x => x.Name == name) != null;
    }
    public static async Task<MTGCardDeck> LoadDeck(string name)
    {
      using var db = new CardDatabaseContext();
      var deck = db.MTGCardDecks.Where(x => x.Name == name).FirstOrDefault();
      if (deck != null)
      {
        await db.Entry(deck).Collection(x => x.DeckCards).LoadAsync();
        await db.Entry(deck).Collection(x => x.Maybelist).LoadAsync();
        await db.Entry(deck).Collection(x => x.Wishlist).LoadAsync();
        await App.CardAPI.PopulateMTGCardInfosAsync(deck.DeckCards.ToArray());
        await App.CardAPI.PopulateMTGCardInfosAsync(deck.Maybelist.ToArray());
        await App.CardAPI.PopulateMTGCardInfosAsync(deck.Wishlist.ToArray());
      }

      return deck;
    }
    public static async Task<MTGCardDeck> SaveDeck(MTGCardDeck deck, string name)
    {
      using var db = new CardDatabaseContext();
      MTGCardDeck savedDeck = null;

      if (string.IsNullOrEmpty(deck.Name))
      {
        // New deck
        deck.Name = name;
        savedDeck = await Task.Run(() => { return db.Add(deck).Entity; });
      }
      else if (deck.Name == name)
      {
        // Same deck
        // Remove from the database the cards that are no longer in the deck
        savedDeck = await Task.Run(() =>
        {
          List<int> validCardIds = new();
          validCardIds.AddRange(deck.DeckCards.Select(x => x.MTGCardId).ToList());
          validCardIds.AddRange(deck.Wishlist.Select(x => x.MTGCardId).ToList());
          validCardIds.AddRange(deck.Maybelist.Select(x => x.MTGCardId).ToList());

          List<MTGCard> missingCards = new();
          missingCards.AddRange(db.MTGCards.Where(card => card.MTGCardDeckDeckCardsId == deck.MTGCardDeckId && !validCardIds.Contains(card.MTGCardId)).ToList());
          missingCards.AddRange(db.MTGCards.Where(card => card.MTGCardDeckWishlistId == deck.MTGCardDeckId && !validCardIds.Contains(card.MTGCardId)).ToList());
          missingCards.AddRange(db.MTGCards.Where(card => card.MTGCardDeckMaybelistId == deck.MTGCardDeckId && !validCardIds.Contains(card.MTGCardId)).ToList());

          db.MTGCards.RemoveRange(missingCards);
          return db.Update(deck).Entity;
        });
      }
      else
      {
        // New deck, keep old deck (Rename)
        savedDeck = await Task.Run(() =>
        {
          MTGCardDeck newDeck = new();
          foreach (var item in deck.DeckCards) { newDeck.DeckCards.Add(new(item.Info, item.Count)); }
          foreach (var item in deck.Wishlist) { newDeck.Wishlist.Add(new(item.Info, item.Count)); }
          foreach (var item in deck.Maybelist) { newDeck.Maybelist.Add(new(item.Info, item.Count)); }
          newDeck.Name = name;
          return db.Update(newDeck).Entity;
        });
      }

      return await db.SaveChangesAsync() > 0 ? savedDeck : null;
    }
    public static async Task<bool> DeleteDeck(MTGCardDeck deck)
    {
      using var db = new CardDatabaseContext();

      db.Remove(deck);
      return await db.SaveChangesAsync() > 0;
    }
  }
}
