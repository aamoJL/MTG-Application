using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MTGApplication.Features.DeckEditor.Commanders.Services;
using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplication.Features.DeckEditor.Editor.Services;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplication.General.Services.ReversibleCommandService;
using MTGApplication.General.ViewModels;
using System;
using static MTGApplication.Features.DeckEditor.CommanderViewModelCommands;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplication.Features.DeckEditor;

public partial class CommanderViewModel(MTGCardImporter importer) : ViewModelBase
{
  public MTGCardImporter Importer { get; } = importer;
  public DeckEditorMTGCardCopier CardCopier { get; } = new();
  public CommanderConfirmers Confirmers { get; init; } = new();
  public ReversibleCommandStack UndoStack { get; init; } = new();
  public IWorker Worker { get; init; } = new DefaultWorker();
  public Notifier Notifier { get; init; } = new();

  [ObservableProperty] private DeckEditorMTGCard card;

  public Action<DeckEditorMTGCard> OnChange { get; init; }

  public IAsyncRelayCommand<DeckEditorMTGCard> ChangeCommanderCommand => new ChangeCommander(this).Command;
  public IRelayCommand<DeckEditorMTGCard> RemoveCommanderCommand => new RemoveCommander(this).Command;
  public IRelayCommand<DeckEditorMTGCard> BeginMoveFromCommand => new MoveCard.BeginMoveFrom(this).Command;
  public IAsyncRelayCommand<DeckEditorMTGCard> BeginMoveToCommand => new MoveCard.BeginMoveTo(this).Command;
  public IRelayCommand<DeckEditorMTGCard> ExecuteMoveCommand => new MoveCard.ExecuteMove(this).Command;
  public IAsyncRelayCommand<string> ImportCommanderCommand => new ImportCommander(this).Command;
  public IAsyncRelayCommand ChangeCardPrintCommand => new ChangeCardPrint(this).Command;
}

