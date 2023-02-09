using CommunityToolkit.Mvvm.ComponentModel;
using MTGApplication.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using static MTGApplication.Models.MTGCardDeck;

namespace MTGApplication.ViewModels
{
  public partial class DeckBuilderViewModel
  {
    public partial class MTGDeckBuilder : ObservableObject
    {
      public MTGDeckBuilder()
      {
        PropertyChanged += MTGDeckBuilder_PropertyChanged;
        CardDeck.DeckCards.CollectionChanged += Cardlist_CollectionChanged;
        CardDeck.Wishlist.CollectionChanged += Wishlist_CollectionChanged;
        CardDeck.Maybelist.CollectionChanged += Maybelist_CollectionChanged;
        CardDeck.PropertyChanged += CardDeck_PropertyChanged;
        DeckCardsViewModels.CollectionChanged += DecklistViewModels_CollectionChanged;
        WishlistViewModels.CollectionChanged += WishlistViewModels_CollectionChanged;
        MaybelistViewModels.CollectionChanged += MaybelistViewModels_CollectionChanged;
      }

      private async void MaybelistViewModels_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
      {
        switch (e.Action)
        {
          case NotifyCollectionChangedAction.Add:
            await SortCardViewModels(MaybelistViewModels, SelectedSortProperty, SelectedSortDirection); break;
          default: break;
        }
      }
      private async void WishlistViewModels_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
      {
        switch (e.Action)
        {
          case NotifyCollectionChangedAction.Add:
            await SortCardViewModels(WishlistViewModels, SelectedSortProperty, SelectedSortDirection); break;
          default: break;
        }
      }
      private async void DecklistViewModels_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
      {
        switch (e.Action)
        {
          case NotifyCollectionChangedAction.Add:
            await SortCardViewModels(DeckCardsViewModels, SelectedSortProperty, SelectedSortDirection); break;
          default: break;
        }
      }
      private void CardDeck_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
      {
        if (e.PropertyName == nameof(MTGCardDeck.DeckSize)) { HasUnsavedChanges = true; }
        if (e.PropertyName == nameof(MTGCardDeck.WishlistSize)) { HasUnsavedChanges = true; }
        if (e.PropertyName == nameof(MTGCardDeck.MaybelistSize)) { HasUnsavedChanges = true; }
      }
      private void MTGDeckBuilder_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
      {
        if (e.PropertyName == nameof(CardDeck))
        {
          CardDeck.DeckCards.CollectionChanged += Cardlist_CollectionChanged;
          CardDeck.Wishlist.CollectionChanged += Wishlist_CollectionChanged;
          CardDeck.Maybelist.CollectionChanged += Maybelist_CollectionChanged;
          CardDeck.PropertyChanged += CardDeck_PropertyChanged;

          DeckCardsViewModels.Clear();
          foreach (var item in CardDeck.DeckCards)
          {
            DeckCardsViewModels.Add(new(item));
          }

          WishlistViewModels.Clear();
          foreach (var item in CardDeck.Wishlist)
          {
            WishlistViewModels.Add(new(item));
          }

          MaybelistViewModels.Clear();
          foreach (var item in CardDeck.Maybelist)
          {
            MaybelistViewModels.Add(new(item));
          }
        }
      }
      private void Maybelist_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
      {
        // Sync Maybelist and MaybelistViewModels
        switch (e.Action)
        {
          case NotifyCollectionChangedAction.Add:
            MaybelistViewModels.Add(new(e.NewItems[0] as MTGCard));
            break;
          case NotifyCollectionChangedAction.Remove:
            MaybelistViewModels.Remove(MaybelistViewModels.FirstOrDefault(x => x.Model == e.OldItems[0])); break;
          case NotifyCollectionChangedAction.Reset:
            MaybelistViewModels.Clear();
            break;
          case NotifyCollectionChangedAction.Move:
            MaybelistViewModels.Move(e.OldStartingIndex, e.NewStartingIndex);
            break;
          default:
            break;
        }
        HasUnsavedChanges = true;
      }
      private void Wishlist_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
      {
        // Sync Wishlist and WishlistViewModels
        switch (e.Action)
        {
          case NotifyCollectionChangedAction.Add:
            WishlistViewModels.Add(new(e.NewItems[0] as MTGCard));
            break;
          case NotifyCollectionChangedAction.Remove:
            WishlistViewModels.Remove(WishlistViewModels.FirstOrDefault(x => x.Model == e.OldItems[0])); break;
          case NotifyCollectionChangedAction.Reset:
            WishlistViewModels.Clear();
            break;
          case NotifyCollectionChangedAction.Move:
            WishlistViewModels.Move(e.OldStartingIndex, e.NewStartingIndex);
            break;
          default:
            break;
        }
        HasUnsavedChanges = true;
      }
      private void Cardlist_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
      {
        // Sync cardlist and cardlistviewmodels
        switch (e.Action)
        {
          case NotifyCollectionChangedAction.Add:
            DeckCardsViewModels.Add(new(e.NewItems[0] as MTGCard));
            break;
          case NotifyCollectionChangedAction.Remove:
            DeckCardsViewModels.Remove(DeckCardsViewModels.FirstOrDefault(x => x.Model == e.OldItems[0])); break;
          case NotifyCollectionChangedAction.Reset:
            DeckCardsViewModels.Clear();
            break;
          case NotifyCollectionChangedAction.Move:
            DeckCardsViewModels.Move(e.OldStartingIndex, e.NewStartingIndex);
            break;
          default:
            break;
        }
        HasUnsavedChanges = true;
      }

      [ObservableProperty]
      private MTGCardDeck cardDeck = new();
      [ObservableProperty]
      private bool isBusy;
      
      public bool HasUnsavedChanges { get; set; }
      public SortProperty SelectedSortProperty { get; set; } = SortProperty.CMC;
      public SortDirection SelectedSortDirection { get; set; } = SortDirection.ASC;
      public ObservableCollection<MTGCardViewModel> DeckCardsViewModels { get; set; } = new();
      public ObservableCollection<MTGCardViewModel> WishlistViewModels { get; set; } = new();
      public ObservableCollection<MTGCardViewModel> MaybelistViewModels { get; set; } = new();

      /// <summary>
      /// Clears current card deck
      /// </summary>
      public void NewDeck()
      {
        IsBusy = true;
        CardDeck = new();
        HasUnsavedChanges = false;
        IsBusy = false;
      }
      
      /// <summary>
      /// Saves current card deck to the database
      /// </summary>
      /// <param name="saveName">Deck name</param>
      public async Task SaveDeck(string saveName)
      {
        IsBusy = true;
        if(await MTGCardDeck.SaveDeck(CardDeck, saveName) is MTGCardDeck savedDeck)
        {
          CardDeck = savedDeck;
          HasUnsavedChanges = false;
          Notifications.RaiseNotification(Notifications.NotificationType.Success, "Deck saved successfully");
        }
        else { Notifications.RaiseNotification(Notifications.NotificationType.Error, "Could not save the deck."); }
        IsBusy = false;
      }
      
      /// <summary>
      /// Replaces current card deck with saved deck from the database
      /// </summary>
      /// <param name="loadName"></param>
      public async Task LoadDeck(string loadName)
      {
        IsBusy = true;
        var newDeck = await Task.Run(() => MTGCardDeck.LoadDeck(loadName));
        if(newDeck != null)
        {
          CardDeck = newDeck;
          HasUnsavedChanges = false;
          Notifications.RaiseNotification(Notifications.NotificationType.Success, "Deck loaded successfully.");
        }
        else { Notifications.RaiseNotification(Notifications.NotificationType.Error, "Could not load the deck."); }
        IsBusy = false;
      }

      /// <summary>
      /// Deletes current card deck from the database
      /// </summary>
      public async Task DeleteDeck()
      {
        IsBusy = true;
        if(await Task.Run(() => MTGCardDeck.DeleteDeck(CardDeck))) { NewDeck(); Notifications.RaiseNotification(Notifications.NotificationType.Success, "Deck deleted successfully."); }
        else { Notifications.RaiseNotification(Notifications.NotificationType.Error, "Could not delete the deck.");}
        IsBusy = false;
      }
      
      /// <summary>
      /// Sorts current card viewmodel lists using selected property and direction
      /// </summary>
      public async Task SortDeck()
      {
        IsBusy = true;
        await SortCardViewModels(SelectedSortProperty, SelectedSortDirection);
        IsBusy = false;
      }

      /// <summary>
      /// Adds cards to cardlist from the given text using card API
      /// </summary>
      public async Task ImportCards(CardlistType cardlistType, string importText)
      {
        IsBusy = true;
        if (!string.IsNullOrEmpty(importText))
        {
          var cards = await App.CardAPI.FetchImportedCards(importText);
          foreach (var item in cards)
          {
            CardDeck.AddToCardlist(cardlistType, item);
          }
          // TODO: not found count
          Notifications.RaiseNotification(Notifications.NotificationType.Success, $"{cards.Length} cards imported successfully.");
        }
        IsBusy = false;
      }

      /// <summary>
      /// Sorts MTGCArdViewModel collections by given <paramref name="prop"/> and <paramref name="dir"/>
      /// </summary>
      public async Task SortCardViewModels(SortProperty prop, SortDirection dir)
      {
        List<Task> tasks = new()
        {
          SortCardViewModels(DeckCardsViewModels, prop, dir),
          SortCardViewModels(WishlistViewModels, prop, dir),
          SortCardViewModels(MaybelistViewModels, prop, dir),
        };

        await Task.WhenAll(tasks);
      }
      private static async Task SortCardViewModels(ObservableCollection<MTGCardViewModel> viewModels, SortProperty prop, SortDirection dir)
      {
        List<MTGCardViewModel> tempList = new();
        tempList = await Task.Run(() => prop switch
        {
          SortProperty.CMC => dir == SortDirection.ASC ? viewModels.OrderBy(x => x.Model.Info.CMC).ToList() : viewModels.OrderByDescending(x => x.Model.Info.CMC).ToList(),
          SortProperty.Name => dir == SortDirection.ASC ? viewModels.OrderBy(x => x.Model.Info.Name).ToList() : viewModels.OrderByDescending(x => x.Model.Info.Name).ToList(),
          SortProperty.Rarity => dir == SortDirection.ASC ? viewModels.OrderBy(x => x.Model.Info.RarityType).ToList() : viewModels.OrderByDescending(x => x.Model.Info.RarityType).ToList(),
          SortProperty.Color => dir == SortDirection.ASC ? viewModels.OrderBy(x => x.Model.Info.ColorType).ToList() : viewModels.OrderByDescending(x => x.Model.Info.ColorType).ToList(),
          SortProperty.Set => dir == SortDirection.ASC ? viewModels.OrderBy(x => x.Model.Info.SetName).ToList() : viewModels.OrderByDescending(x => x.Model.Info.SetName).ToList(),
          SortProperty.Count => dir == SortDirection.ASC ? viewModels.OrderBy(x => x.Model.Count).ToList() : viewModels.OrderByDescending(x => x.Model.Count).ToList(),
          SortProperty.Price => dir == SortDirection.ASC ? viewModels.OrderBy(x => x.Model.Info.Price).ToList() : viewModels.OrderByDescending(x => x.Model.Info.Price).ToList(),
          _ => throw new NotImplementedException(),
        });

        for (int i = 0; i < tempList.Count; i++)
        {
          viewModels.Move(viewModels.IndexOf(tempList[i]), i);
        }
      }
    }
  }
}
