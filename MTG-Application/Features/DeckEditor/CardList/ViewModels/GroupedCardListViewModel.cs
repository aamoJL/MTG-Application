using CommunityToolkit.Mvvm.Input;
using MTGApplication.Features.DeckEditor.CardList.Services;
using MTGApplication.Features.DeckEditor.CardList.Services.Factories;
using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplication.General.Services.IOServices;
using MTGApplication.General.Services.ReversibleCommandService;
using MTGApplication.General.ViewModels;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using static MTGApplication.Features.DeckEditor.CardList.UseCases.CardListViewModelCommands;
using static MTGApplication.Features.DeckEditor.CardList.UseCases.GroupedCardListViewModelCommands;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplication.Features.DeckEditor.ViewModels;

public partial class GroupedCardListViewModel : IGroupedCardListViewModel
{
  public GroupedCardListViewModel(ObservableCollection<DeckEditorMTGCard> cards, IMTGCardImporter importer)
  {
    Cards = cards;
    Importer = importer;
  }

  public ObservableCollection<DeckEditorMTGCard> Cards
  {
    get;
    private init
    {
      field = value;

      if (field is INotifyCollectionChanged observableSource)
        observableSource.CollectionChanged += Source_CollectionChanged;

      field.CollectionChanged += Source_CollectionChanged;
    }
  }

  [NotNull] public ObservableCollection<CardGroupViewModel>? Groups => field ??= InitGroups(Cards);

  public IMTGCardImporter Importer { get; }

  public ReversibleCommandStack UndoStack { get; init; } = new();
  public ClipboardService ClipboardService { get; init; } = new();
  public GroupedCardListConfirmers Confirmers { get; init; } = new();
  public Notifier Notifier { get; init; } = new();
  public IWorker Worker { get; init; } = IWorker.Default;

  [NotNull] public IAsyncRelayCommand? AddGroupCommand => field ??= new AddCardGroup(this).Command;
  [NotNull] public IRelayCommand<CardGroupViewModel>? RemoveGroupCommand => field ??= new RemoveCardGroup(this).Command;
  [NotNull] public IAsyncRelayCommand<CardGroupViewModel>? RenameGroupCommand => field ??= new RenameCardGroup(this).Command;

  [NotNull] public IAsyncRelayCommand<DeckEditorMTGCard>? AddCardCommand => field ??= new AddCard(Cards, UndoStack, Confirmers).Command;
  [NotNull] public IRelayCommand<DeckEditorMTGCard>? RemoveCardCommand => field ??= new RemoveCard(Cards, UndoStack).Command;
  [NotNull] public IRelayCommand<DeckEditorMTGCard>? BeginMoveFromCommand => field ??= new MoveCard.BeginMoveFrom(Cards, UndoStack).Command;
  [NotNull] public IAsyncRelayCommand<DeckEditorMTGCard>? BeginMoveToCommand => field ??= new MoveCard.BeginMoveTo(Cards, UndoStack, Confirmers).Command;
  [NotNull] public IRelayCommand<DeckEditorMTGCard>? ExecuteMoveCommand => field ??= new MoveCard.ExecuteMove(UndoStack).Command;
  [NotNull] public IRelayCommand? ClearCommand => field ??= new Clear(Cards, UndoStack).Command;
  [NotNull] public IAsyncRelayCommand<string>? ImportCardsCommand => field ??= new ImportCards(Cards, UndoStack, Confirmers, Worker, Importer, Notifier).Command;
  [NotNull] public IAsyncRelayCommand<string>? ExportCardsCommand => field ??= new ExportCards(Cards, Confirmers, Notifier, ClipboardService).Command;

  private void Source_CollectionChanged(object? _, NotifyCollectionChangedEventArgs e)
  {
    if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems is IList newItems)
    {
      // Add missing groups
      foreach (var item in newItems.OfType<DeckEditorMTGCard>()
        .DistinctBy(g => g.Group)
        .Where(c => Groups.FirstOrDefault(g => g.Key == c.Group) is null))
      {
        Groups.Add(new GroupedCardListCardGroupFactory(this).CreateCardGroup(item.Group));
      }
    }
  }

  private ObservableCollection<CardGroupViewModel> InitGroups(ObservableCollection<DeckEditorMTGCard> cards)
  {
    var groups = new ObservableCollection<CardGroupViewModel>();

    foreach (var group in cards
        .Select(c => c.Group)
        .Where(g => g != string.Empty)
        .Distinct()
        .Order())
    {
      groups.Add(new GroupedCardListCardGroupFactory(this).CreateCardGroup(group));
    }

    groups.Add(new GroupedCardListCardGroupFactory(this).CreateCardGroup(string.Empty));

    return groups;
  }
}