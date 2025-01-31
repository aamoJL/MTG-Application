using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MTGApplication.Features.DeckEditor.Commanders.Services;
using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplication.General.Services.ReversibleCommandService;
using MTGApplication.General.ViewModels;
using System.Diagnostics.CodeAnalysis;
using static MTGApplication.Features.DeckEditor.Commanders.UseCases.CommanderViewModelCommands;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplication.Features.DeckEditor.Commanders.ViewModels;

public partial class CommanderViewModel(IMTGCardImporter importer) : ObservableObject
{
  public DeckEditorMTGCard? Card
  {
    get;
    set => SetProperty(ref field, value);
  }

  public IMTGCardImporter Importer { get; } = importer;
  public CommanderConfirmers Confirmers { get; init; } = new();
  public ReversibleCommandStack UndoStack { get; init; } = new();
  public Notifier Notifier { get; init; } = new();
  public IWorker Worker { get; init; } = IWorker.Default;

  [NotNull] public IAsyncRelayCommand<DeckEditorMTGCard>? ChangeCommanderCommand => field ??= new ChangeCommander(this).Command;
  [NotNull] public IRelayCommand<DeckEditorMTGCard>? RemoveCommanderCommand => field ??= new RemoveCommander(this).Command;
  [NotNull] public IRelayCommand<DeckEditorMTGCard>? BeginMoveFromCommand => field ??= new MoveCard.BeginMoveFrom(this).Command;
  [NotNull] public IAsyncRelayCommand<DeckEditorMTGCard>? BeginMoveToCommand => field ??= new MoveCard.BeginMoveTo(this).Command;
  [NotNull] public IRelayCommand? ExecuteMoveCommand => field ??= new MoveCard.ExecuteMove(this).Command;
  [NotNull] public IAsyncRelayCommand<string>? ImportCommanderCommand => field ??= new ImportCommander(this).Command;
}

