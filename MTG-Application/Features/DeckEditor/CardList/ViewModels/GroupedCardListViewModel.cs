using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MTGApplication.Features.DeckEditor.CardList.Services;
using MTGApplication.Features.DeckEditor.CardList.UseCases;
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

  public void OnChange() => OnPropertyChanged(nameof(Count));

  private void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    => OnChange();
}

public partial class GroupedCardListViewModel : CardListViewModel
{
  public GroupedCardListViewModel(MTGCardImporter importer, Func<DeckEditorMTGCard, string> groupBy, GroupedCardListConfirmers confirmers = null) : base(importer, confirmers)
  {
    Commands = new(this);
    Confirmers = confirmers ?? new();

    GetItemKey = groupBy;

    PropertyChanging += GroupedCardListViewModel_PropertyChanging;
    PropertyChanged += GroupedCardListViewModel_PropertyChanged;
  }

  public override GroupedCardListConfirmers Confirmers { get; }

  public ObservableCollection<CardGroup> Groups { get; } = [new(string.Empty)];

  private Func<DeckEditorMTGCard, string> GetItemKey { get; }

  protected override GroupedCardListViewModelCommands Commands { get; }

  public IAsyncRelayCommand AddCardGroupCommand => Commands.AddCardGroupCommand;

  public override void OnCardChange(DeckEditorMTGCard card)
  {
    base.OnCardChange(card);

    Groups.FirstOrDefault(x => x.Key == card.Group)?.OnChange();
  }

  private void ResetGroups()
  {
    Groups.Clear();
    Groups.Add(new(string.Empty));
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