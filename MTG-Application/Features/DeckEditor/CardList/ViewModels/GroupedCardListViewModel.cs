﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MTGApplication.Features.DeckEditor.CardList.Services;
using MTGApplication.Features.DeckEditor.CardList.UseCases;
using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplication.General.Services.Importers.CardImporter;
using System.Collections.ObjectModel;
using System.Linq;
using static MTGApplication.Features.DeckEditor.CardList.UseCases.ReversibleActions.CardListViewModelReversibleActions;

namespace MTGApplication.Features.DeckEditor.ViewModels;

public partial class CardGroupViewModel : ObservableObject
{
  public CardGroupViewModel(string key, GroupedCardListViewModel listViewmodel)
  {
    Key = key;
    Commands = new(this, listViewmodel);

    Items.CollectionChanged += Items_CollectionChanged;
  }

  [ObservableProperty] public partial string Key { get; set; }

  public ObservableCollection<DeckEditorMTGCard> Items { get; } = [];
  public int Count => Items.Sum(x => x.Count);

  public void OnChange() => OnPropertyChanged(nameof(Count));

  public CardGroupViewModelCommands Commands { get; }

  public IAsyncRelayCommand<DeckEditorMTGCard> AddCardToGroupCommand => Commands.AddCardToGroupCommand;
  public IRelayCommand<DeckEditorMTGCard> BeginMoveFromCommand => Commands.BeginMoveFromCommand;
  public IAsyncRelayCommand<DeckEditorMTGCard> BeginMoveToCommand => Commands.BeginMoveToCommand;
  public IRelayCommand<DeckEditorMTGCard> ExecuteMoveCommand => Commands.ExecuteMoveCommand;
  public IAsyncRelayCommand RenameGroupCommand => Commands.RenameGroupCommand;
  public IAsyncRelayCommand<string> ImportCardsToGroupCommand => Commands.ImportCardsToGroupCommand;

  private void Items_CollectionChanged(object? _, System.Collections.Specialized.NotifyCollectionChangedEventArgs __)
    => OnChange();
}

public partial class GroupedCardListViewModel : CardListViewModel
{
  public GroupedCardListViewModel(MTGCardImporter importer, GroupedCardListConfirmers? confirmers = null) : base(importer, confirmers)
  {
    Commands = new(this);
    Confirmers ??= confirmers ?? new();

    PropertyChanging += GroupedCardListViewModel_PropertyChanging;
    PropertyChanged += GroupedCardListViewModel_PropertyChanged;

    new ReversibleAddGroupAction(this).Action?.Invoke(string.Empty);
  }

  public override GroupedCardListConfirmers Confirmers { get; }
  public ObservableCollection<CardGroupViewModel> Groups { get; } = [];

  private GroupedCardListViewModelCommands Commands { get; }

  public IAsyncRelayCommand AddGroupCommand => Commands.AddGroupCommand;
  public IRelayCommand RemoveGroupCommand => Commands.RemoveGroupCommand;

  public override void OnCardChange(DeckEditorMTGCard card, string property)
  {
    base.OnCardChange(card, property);

    switch (property)
    {
      case nameof(card.Count):
        Groups.FirstOrDefault(x => x.Key == card.Group)?.OnChange();
        break;
      default: break;
    }
  }

  private void GroupedCardListViewModel_PropertyChanging(object? _, System.ComponentModel.PropertyChangingEventArgs e)
  {
    if (e.PropertyName == nameof(Cards))
      Cards.CollectionChanged -= Cards_CollectionChanged;
  }

  private void GroupedCardListViewModel_PropertyChanged(object? _, System.ComponentModel.PropertyChangedEventArgs e)
  {
    if (e.PropertyName == nameof(Cards))
    {
      Groups.Clear();
      var emptyGroup = new CardGroupViewModel(string.Empty, this);
      Groups.Add(emptyGroup);

      foreach (var card in Cards)
      {
        var key = card.Group;
        var group = Groups.FirstOrDefault(x => key == x.Key);

        if (group == null)
        {
          var addAction = new ReversibleAddGroupAction(this);
          addAction.Action?.Invoke(key);
          group = addAction.Group;
        }

        group?.Items.Add(card);
      }

      Cards.CollectionChanged += Cards_CollectionChanged;
    }
  }

  private void Cards_CollectionChanged(object? _, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
  {
    if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add
      && e.NewItems?[0] is DeckEditorMTGCard newCard)
    {
      var key = newCard.Group;
      var group = Groups.FirstOrDefault(x => key == x.Key);

      if (group == null)
      {
        var addAction = new ReversibleAddGroupAction(this);
        addAction.Action?.Invoke(key);
        group = addAction.Group;
      }

      group?.Items.Add(newCard);
    }
    else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove
      && e.OldItems?[0] is DeckEditorMTGCard oldCard)
    {
      var key = oldCard.Group;

      Groups.FirstOrDefault(x => key == x.Key)?.Items.Remove(oldCard);
    }
  }
}