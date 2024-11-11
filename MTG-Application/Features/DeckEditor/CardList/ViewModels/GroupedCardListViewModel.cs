using CommunityToolkit.Mvvm.ComponentModel;
using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplication.General.Services.Importers.CardImporter;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace MTGApplication.Features.DeckEditor.ViewModels;

public partial class CardGroup : ObservableObject
{
  public CardGroup(string key)
  {
    Key = key;

    Items.CollectionChanged += Items_CollectionChanged;
  }

  public ObservableCollection<DeckEditorMTGCard> Items { get; } = [];
  public string Key { get; }

  public int Count => Items.Sum(x => x.Count);

  private void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    => OnPropertyChanged(nameof(Count));
}

public partial class GroupedCardListViewModel : CardListViewModel
{
  public GroupedCardListViewModel(MTGCardImporter importer, Func<DeckEditorMTGCard, string> groupBy) : base(importer)
  {
    GetItemKey = groupBy;

    PropertyChanging += GroupedCardListViewModel_PropertyChanging;
    PropertyChanged += GroupedCardListViewModel_PropertyChanged;
  }

  public ObservableCollection<CardGroup> Groups { get; } = [new(string.Empty)];

  private Func<DeckEditorMTGCard, string> GetItemKey { get; }

  private void ResetGroups()
  {
    Groups.Clear();
    AddGroup(string.Empty);
  }

  private void AddGroup(string key)
  {
    var index = Groups.IndexOf(Groups.FirstOrDefault(x =>
    {
      // Empty key will always be the last item
      return x.Key == string.Empty || x.Key.CompareTo(key) >= 0;
    }));

    if (index >= 0)
      Groups.Insert(index, new(key));
    else
      Groups.Add(new(key));
  }

  private void GroupedCardListViewModel_PropertyChanging(object sender, System.ComponentModel.PropertyChangingEventArgs e)
  {
    if (e.PropertyName == nameof(Cards))
    {
      ResetGroups();

      Cards.CollectionChanged -= Cards_CollectionChanged;
    }
  }

  private void GroupedCardListViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
  {
    if (e.PropertyName == nameof(Cards))
    {
      foreach (var card in Cards)
      {
        var key = GetItemKey.Invoke(card);
        var group = Groups.FirstOrDefault(x => key == x.Key);

        if (group == null)
          Groups.Add(group = new(key));

        group.Items.Add(card);
      }

      Cards.CollectionChanged += Cards_CollectionChanged;
    }
  }

  private void Cards_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
  {
    if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
    {
      var key = GetItemKey.Invoke(e.NewItems[0] as DeckEditorMTGCard);
      var group = Groups.FirstOrDefault(x => key == x.Key);

      if (group == null)
        Groups.Add(group = new(key));

      group.Items.Add(e.NewItems[0] as DeckEditorMTGCard);
    }
    else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
    {
      var key = GetItemKey.Invoke(e.OldItems[0] as DeckEditorMTGCard);

      Groups.FirstOrDefault(x => key == x.Key)
        ?.Items.Remove(e.OldItems[0] as DeckEditorMTGCard);
    }
    else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
      ResetGroups();
  }
}