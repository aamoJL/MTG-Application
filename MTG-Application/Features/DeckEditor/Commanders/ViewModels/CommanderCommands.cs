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

public partial class CommanderCommands(DeckEditorViewModel viewmodel, CommanderCommands.CommanderType commanderType)
{
  public enum CommanderType { Commander, Partner }

  public MTGCardImporter Importer { get; } = viewmodel.Importer;
  public DeckEditorMTGCardCopier CardCopier { get; } = new();
  public CommanderConfirmers Confirmers { get; init; } = viewmodel.Confirmers.CommanderConfirmers;
  public ReversibleCommandStack UndoStack { get; init; } = viewmodel.UndoStack;
  public IWorker Worker { get; init; } = viewmodel;
  public Notifier Notifier { get; init; } = viewmodel.Notifier;

  public Action<DeckEditorMTGCard> OnChange { get; init; }

  public IAsyncRelayCommand<DeckEditorMTGCard> ChangeCommanderCommand => (changeCommander ??= new ChangeCommander(this)).Command;
  public IRelayCommand<DeckEditorMTGCard> RemoveCommanderCommand => (removeCommander ??= new RemoveCommander(this)).Command;
  public IRelayCommand<DeckEditorMTGCard> BeginMoveFromCommand => (beginMoveFrom ??= new MoveCard.BeginMoveFrom(this)).Command;
  public IAsyncRelayCommand<DeckEditorMTGCard> BeginMoveToCommand => (beginMoveTo ??= new MoveCard.BeginMoveTo(this)).Command;
  public IRelayCommand<DeckEditorMTGCard> ExecuteMoveCommand => (executeMove ??= new MoveCard.ExecuteMove(this)).Command;
  public IAsyncRelayCommand<string> ImportCommanderCommand => (importCommander ??= new ImportCommander(this)).Command;
  public IAsyncRelayCommand ChangeCardPrintCommand => (changeCardPrint ??= new ChangeCardPrint(this)).Command;

  private ChangeCommander changeCommander;
  private RemoveCommander removeCommander;
  private MoveCard.BeginMoveFrom beginMoveFrom;
  private MoveCard.BeginMoveTo beginMoveTo;
  private MoveCard.ExecuteMove executeMove;
  private ImportCommander importCommander;
  private ChangeCardPrint changeCardPrint;

  public DeckEditorMTGCard GetCommander() => commanderType == CommanderType.Commander ? viewmodel.Commander : viewmodel.Partner;
}

