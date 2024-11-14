using CommunityToolkit.Mvvm.Input;
using MTGApplication.Features.DeckEditor.CardList.Services;
using MTGApplication.Features.DeckEditor.CardList.UseCases;
using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplication.General.ViewModels;
using System.Collections.ObjectModel;
using System.Linq;

namespace MTGApplication.Features.DeckEditor.ViewModels;

public partial class CardGroupViewModel : ViewModelBase
{
  public CardGroupViewModel(string key)
  {
    Key = key;

    Items.CollectionChanged += Items_CollectionChanged;
  }

  public ObservableCollection<DeckEditorMTGCard> Items { get; } = [];
  public string Key { get; }
  public int Count => Items.Sum(x => x.Count);

  public void OnChange() => OnPropertyChanged(nameof(Count));

  public CardGroupViewModelCommands Commands { get; set; }

  public IAsyncRelayCommand<DeckEditorMTGCard> AddCardToGroupCommand => Commands?.AddCardToGroupCommand;
  public IRelayCommand<DeckEditorMTGCard> BeginMoveFromCommand => Commands?.BeginMoveFromCommand;
  public IAsyncRelayCommand<DeckEditorMTGCard> BeginMoveToCommand => Commands?.BeginMoveToCommand;
  public IRelayCommand<DeckEditorMTGCard> ExecuteMoveCommand => Commands?.ExecuteMoveCommand;

  private void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    => OnChange();
}

public partial class GroupedCardListViewModel : CardListViewModel
{
  public GroupedCardListViewModel(MTGCardImporter importer, GroupedCardListConfirmers confirmers = null) : base(importer, confirmers)
  {
    Commands = new(this);
    Confirmers ??= confirmers;

    PropertyChanging += GroupedCardListViewModel_PropertyChanging;
    PropertyChanged += GroupedCardListViewModel_PropertyChanged;

    AddNewGroup(string.Empty);
  }

  public override GroupedCardListConfirmers Confirmers { get; }
  public ObservableCollection<CardGroupViewModel> Groups { get; } = [];

  private GroupedCardListViewModelCommands Commands { get; }

  public IAsyncRelayCommand AddGroupCommand => Commands.AddGroupCommand;
  public IRelayCommand RemoveGroupCommand => Commands.RemoveGroupCommand;

  public override void OnCardChange(DeckEditorMTGCard card, string property)
  {
    base.OnCardChange(card, property);

    var key = card.Group;
    var group = Groups.FirstOrDefault(x => x.Key == key);

    switch (property)
    {
      case nameof(card.Group):
        // remove card if exists in the old group
        if (Groups.FirstOrDefault(x => x.Items.Contains(card)) is CardGroupViewModel found)
          found.Items.Remove(card);

        (group ??= AddNewGroup(key)).Items.Add(card);
        break;
      case nameof(card.Info): break;
      default: group?.OnChange(); break;
    }
  }

  public CardGroupViewModel AddNewGroup(string key)
  {
    // Find the alphabetical index of the key. Empty key will always be the last item
    var index = Groups.IndexOf(
            Groups.FirstOrDefault(x => x.Key == string.Empty || x.Key.CompareTo(key) >= 0));

    var group = new CardGroupViewModel(key);
    group.Commands = new(group, this);

    if (index >= 0)
      Groups.Insert(index, group);
    else
      Groups.Add(group);

    // Add items from default list of item has the same group key as the new group
    if (group.Key != string.Empty)
    {
      if (Groups.FirstOrDefault(x => x.Key == string.Empty) is CardGroupViewModel defaultGroup)
      {
        foreach (var card in defaultGroup.Items)
        {
          if (card.Group == group.Key)
          {
            defaultGroup.Items.Remove(card);
            group.Items.Add(card);
          }
        }
      }
    }

    return group;
  }

  private void GroupedCardListViewModel_PropertyChanging(object sender, System.ComponentModel.PropertyChangingEventArgs e)
  {
    if (e.PropertyName == nameof(Cards))
      Cards.CollectionChanged -= Cards_CollectionChanged;
  }

  private void GroupedCardListViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
  {
    if (e.PropertyName == nameof(Cards))
    {
      Groups.Clear();
      var emptyGroup = new CardGroupViewModel(string.Empty);
      emptyGroup.Commands = new(emptyGroup, this);
      Groups.Add(emptyGroup);

      foreach (var card in Cards)
      {
        var key = card.Group;
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
      var item = (e.NewItems[0] as DeckEditorMTGCard);
      var key = item.Group;
      var group = Groups.FirstOrDefault(x => key == x.Key);

      (group ??= AddNewGroup(key)).Items.Add(item);
    }
    else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
    {
      var key = (e.OldItems[0] as DeckEditorMTGCard).Group;

      Groups.FirstOrDefault(x => key == x.Key)
        ?.Items.Remove(e.OldItems[0] as DeckEditorMTGCard);
    }
    else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
      AddNewGroup(string.Empty);
  }
}