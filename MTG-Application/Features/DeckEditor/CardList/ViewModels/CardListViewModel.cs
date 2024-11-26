using CommunityToolkit.Mvvm.Input;
using MTGApplication.Features.DeckEditor.CardList.Services;
using MTGApplication.Features.DeckEditor.CardList.UseCases;
using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplication.Features.DeckEditor.Editor.Services;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplication.General.Services.IOServices;
using MTGApplication.General.Services.ReversibleCommandService;
using MTGApplication.General.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using static MTGApplication.Features.DeckEditor.CardList.UseCases.CardListViewModelCommands;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplication.Features.DeckEditor.ViewModels;

public partial class CardListViewModel : INotifyPropertyChanged, INotifyPropertyChanging
{
  public CardListViewModel(MTGCardImporter importer, CardListConfirmers? confirmers = null)
  {
    Commands = new(this);
    Importer = importer;
    Confirmers = confirmers ?? new();
  }

  public ObservableCollection<DeckEditorMTGCard> Cards
  {
    get;
    set
    {
      if (value != field)
      {
        PropertyChanging?.Invoke(this, new(nameof(Cards)));
        field = value;
        PropertyChanged?.Invoke(this, new(nameof(Cards)));
      }
    }
  } = [];

  public event PropertyChangedEventHandler? PropertyChanged;
  public event PropertyChangingEventHandler? PropertyChanging;

  public virtual CardListConfirmers Confirmers { get; }

  public MTGCardImporter Importer { get; }
  public ReversibleCommandStack UndoStack { get; init; } = new();
  public ClipboardService ClipboardService { get; init; } = new();
  public Notifier Notifier { get; init; } = new();
  public IWorker Worker { get; init; } = IWorker.Default;
  public DeckEditorMTGCardCopier CardCopier { get; } = new();
  public CardFilters CardFilters { get; init; } = new();
  public CardSorter CardSorter { get; init; } = new();

  private CardListViewModelCommands Commands { get; }

  public Action? OnChange { private get; init; }

  public IAsyncRelayCommand<DeckEditorMTGCard> AddCardCommand => Commands.AddCardCommand;
  public IRelayCommand<DeckEditorMTGCard> RemoveCardCommand => Commands.RemoveCardCommand;
  public IRelayCommand<DeckEditorMTGCard> BeginMoveFromCommand => Commands.BeginMoveFromCommand;
  public IAsyncRelayCommand<DeckEditorMTGCard> BeginMoveToCommand => Commands.BeginMoveToCommand;
  public IRelayCommand<DeckEditorMTGCard> ExecuteMoveCommand => Commands.ExecuteMoveCommand;
  public IRelayCommand ClearCommand => Commands.ClearCommand;
  public IAsyncRelayCommand<string> ImportCardsCommand => Commands.ImportCardsCommand;
  public IAsyncRelayCommand<string> ExportCardsCommand => Commands.ExportCardsCommand;
  public IRelayCommand<CardCountChangeArgs> ChangeCardCountCommand => Commands.ChangeCardCountCommand;
  public IAsyncRelayCommand<DeckEditorMTGCard> ChangeCardPrintCommand => Commands.ChangeCardPrintCommand;

  public virtual void OnListChange() => OnChange?.Invoke();
  public virtual void OnCardChange(DeckEditorMTGCard card, string property) => OnChange?.Invoke();
}