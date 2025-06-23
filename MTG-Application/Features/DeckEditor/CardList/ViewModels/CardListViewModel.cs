using CommunityToolkit.Mvvm.Input;
using MTGApplication.Features.DeckEditor.CardList.Services;
using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplication.General.Services.IOServices;
using MTGApplication.General.Services.ReversibleCommandService;
using MTGApplication.General.ViewModels;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using static MTGApplication.Features.DeckEditor.CardList.UseCases.CardListViewModelCommands;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplication.Features.DeckEditor.ViewModels;

public partial class CardListViewModel(IMTGCardImporter importer, CardListConfirmers? confirmers = null) : ICardListViewModel, INotifyPropertyChanged, INotifyPropertyChanging
{
  public ObservableCollection<DeckEditorMTGCard> Cards
  {
    get;
    set
    {
      if (field == value)
        return;

      PropertyChanging?.Invoke(this, new(nameof(Cards)));
      field = value;
      PropertyChanged?.Invoke(this, new(nameof(Cards)));
    }
  } = [];

  public IMTGCardImporter Importer { get; } = importer;

  public ReversibleCommandStack UndoStack { get; init; } = new();
  public ClipboardService ClipboardService { get; init; } = new();
  public Notifier Notifier { get; init; } = new();
  public IWorker Worker { get; init; } = IWorker.Default;

  public virtual CardListConfirmers Confirmers { get; } = confirmers ??= new();

  public event PropertyChangedEventHandler? PropertyChanged;
  public event PropertyChangingEventHandler? PropertyChanging;

  [NotNull] public IAsyncRelayCommand<DeckEditorMTGCard>? AddCardCommand => field ??= new AddCard(this).Command;
  [NotNull] public IRelayCommand<DeckEditorMTGCard>? RemoveCardCommand => field ??= new RemoveCard(this).Command;
  [NotNull] public IRelayCommand<DeckEditorMTGCard>? BeginMoveFromCommand => field ??= new MoveCard.BeginMoveFrom(this).Command;
  [NotNull] public IAsyncRelayCommand<DeckEditorMTGCard>? BeginMoveToCommand => field ??= new MoveCard.BeginMoveTo(this).Command;
  [NotNull] public IRelayCommand<DeckEditorMTGCard>? ExecuteMoveCommand => field ??= new MoveCard.ExecuteMove(this).Command;
  [NotNull] public IRelayCommand? ClearCommand => field ??= new Clear(this).Command;
  [NotNull] public IAsyncRelayCommand<string>? ImportCardsCommand => field ??= new ImportCards(this).Command;
  [NotNull] public IAsyncRelayCommand<string>? ExportCardsCommand => field ??= new ExportCards(this).Command;
}