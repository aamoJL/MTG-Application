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

  public IAsyncRelayCommand<DeckEditorMTGCard> ChangeCommanderCommand => changeCommander?.Command ?? (changeCommander = new ChangeCommander(this)).Command;
  public IRelayCommand<DeckEditorMTGCard> RemoveCommanderCommand => removeCommander?.Command ?? (removeCommander = new RemoveCommander(this)).Command;
  public IRelayCommand<DeckEditorMTGCard> BeginMoveFromCommand => beginMoveFrom?.Command ?? (beginMoveFrom = new MoveCard.BeginMoveFrom(this)).Command;
  public IAsyncRelayCommand<DeckEditorMTGCard> BeginMoveToCommand => beginMoveTo?.Command ?? (beginMoveTo = new MoveCard.BeginMoveTo(this)).Command;
  public IRelayCommand<DeckEditorMTGCard> ExecuteMoveCommand => executeMove?.Command ?? (executeMove = new MoveCard.ExecuteMove(this)).Command;
  public IAsyncRelayCommand<string> ImportCommanderCommand => importCommander?.Command ?? (importCommander = new ImportCommander(this)).Command;
  public IAsyncRelayCommand ChangeCardPrintCommand => changeCardPrint?.Command ?? (changeCardPrint = new ChangeCardPrint(this)).Command;

  private ChangeCommander changeCommander;
  private RemoveCommander removeCommander;
  private MoveCard.BeginMoveFrom beginMoveFrom;
  private MoveCard.BeginMoveTo beginMoveTo;
  private MoveCard.ExecuteMove executeMove;
  private ImportCommander importCommander;
  private ChangeCardPrint changeCardPrint;
}

