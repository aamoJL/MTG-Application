using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.WinUI.Helpers;
using MTGApplication.Features.DeckEditor.CardList.Services;
using MTGApplication.Features.DeckEditor.CardList.UseCases;
using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplication.General.Services.IOServices;
using MTGApplication.General.Services.ReversibleCommandService;
using MTGApplication.General.ViewModels;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using static MTGApplication.Features.DeckEditor.CardList.UseCases.CardGroupViewModelCommands;
using static MTGApplication.Features.DeckEditor.CardList.UseCases.GroupedCardListViewModelCommands;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplication.Features.DeckEditor.ViewModels;

public partial class CardGroupViewModel : ObservableObject, ICardListViewModel
{
  public CardGroupViewModel(string key, ObservableCollection<DeckEditorMTGCard> source, IMTGCardImporter importer)
  {
    Key = key;
    Source = source;
    Importer = importer;

    Cards.CollectionChanged += Cards_CollectionChanged;
  }

  [ObservableProperty] public partial string Key { get; set; }
  public ObservableCollection<DeckEditorMTGCard> Cards { get; } = [];
  public IMTGCardImporter Importer { get; }

  public GroupedCardListConfirmers Confirmers { get; init; } = new();
  public ReversibleCommandStack UndoStack { get; init; } = new();
  public ClipboardService ClipboardService { get; init; } = new();
  public Notifier Notifier { get; init; } = new();
  public IWorker Worker { get; init; } = IWorker.Default;

  public int Count => Cards.Sum(x => x.Count);

  public ObservableCollection<DeckEditorMTGCard> Source
  {
    get;
    private set
    {
      if (field == value)
        return;

      if (field is ObservableCollection<DeckEditorMTGCard> old)
      {
        SourceWeakEventListener?.Detach();

        foreach (var item in old)
          item.PropertyChanged -= SourceItem_PropertyChanged;
      }

      SetProperty(ref field, value);

      if (Source is INotifyCollectionChanged observableSource)
      {
        SourceWeakEventListener = new(this)
        {
          OnEventAction = (sender, _, e) => Source_CollectionChanged(sender, e),
          OnDetachAction = (listener) =>
          {
            if (SourceWeakEventListener != null)
              observableSource.CollectionChanged -= SourceWeakEventListener.OnEvent!;
          }
        };

        observableSource.CollectionChanged += SourceWeakEventListener.OnEvent!;
      }

      Cards.Clear();

      foreach (var item in Source)
      {
        if (item.Group == Key)
          Cards.Add(item);

        item.PropertyChanged += SourceItem_PropertyChanged;
      }
    }
  }

  private WeakEventListener<CardGroupViewModel, object, NotifyCollectionChangedEventArgs>? SourceWeakEventListener { get; set; }

  [NotNull] public IAsyncRelayCommand<DeckEditorMTGCard>? AddCardCommand => field ??= new AddCardToGroup(this).Command;
  [NotNull] public IRelayCommand<DeckEditorMTGCard>? RemoveCardCommand => field ??= new RemoveCardFromGroup(this).Command;
  [NotNull] public IRelayCommand<DeckEditorMTGCard>? BeginMoveFromCommand => field ??= new MoveGroupCard.BeginMoveFrom(this).Command;
  [NotNull] public IAsyncRelayCommand<DeckEditorMTGCard>? BeginMoveToCommand => field ??= new MoveGroupCard.BeginMoveTo(this).Command;
  [NotNull] public IRelayCommand<DeckEditorMTGCard>? ExecuteMoveCommand => field ??= new MoveGroupCard.ExecuteMove(this).Command;
  [NotNull] public IRelayCommand? ClearCommand => field ??= new ClearCardGroup(this).Command;
  [NotNull] public IAsyncRelayCommand<string>? ImportCardsCommand => field ??= new ImportCardsToGroup(this).Command;
  [NotNull] public IAsyncRelayCommand<string>? ExportCardsCommand => field ??= new AsyncRelayCommand<string>((_) => throw new NotImplementedException());
  [NotNull] public IRelayCommand<CardListViewModelCommands.CardCountChangeArgs>? ChangeCardCountCommand => field ??= new ChangeCardCount(this).Command;
  [NotNull] public IAsyncRelayCommand<DeckEditorMTGCard>? ChangeCardPrintCommand => field ??= new ChangeCardPrint(this).Command;

  private void Source_CollectionChanged(object? _, NotifyCollectionChangedEventArgs e)
  {
    if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems is IList newItems)
    {
      foreach (var item in newItems.OfType<DeckEditorMTGCard>())
      {
        if (item.Group == Key)
          Cards.Add(item);

        item.PropertyChanged += SourceItem_PropertyChanged;
      }
    }
    else if (e.Action == NotifyCollectionChangedAction.Remove && e.OldItems is IList oldItems)
    {
      foreach (var item in oldItems.OfType<DeckEditorMTGCard>())
      {
        if (item.Group == Key)
          Cards.Remove(item);

        item.PropertyChanged -= SourceItem_PropertyChanged;
      }
    }
  }

  private void SourceItem_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
  {
    if (sender is not DeckEditorMTGCard card)
      return;

    if (e.PropertyName == nameof(DeckEditorMTGCard.Count))
    {
      if (card.Group == Key)
        OnPropertyChanged(nameof(Count));
    }
    else if (e.PropertyName == nameof(DeckEditorMTGCard.Group))
    {
      if (!Source.Contains(card))
        return;

      if (card.Group == Key && !Cards.Contains(card))
        Cards.Add(card);
      else if (card.Group != Key && Cards.Contains(card))
        Cards.Remove(card);
    }
  }

  private void Cards_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    => OnPropertyChanged(nameof(Count));
}
