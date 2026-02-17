using CommunityToolkit.Mvvm.ComponentModel;
using MTGApplication.General.Services.Importers.CardImporter;

namespace MTGApplication.Features.DeckEditor.Commanders.ViewModels;

public partial class CommanderViewModel(IMTGCardImporter importer) : ObservableObject
{
  //public DeckEditorMTGCard? Card
  //{
  //  get;
  //  set => SetProperty(ref field, value);
  //}

  //public IMTGCardImporter Importer { get; } = importer;
  //public IEdhrecImporter EdhrecImporter { get; set; } = new EdhrecImporter();
  //public CommanderConfirmers Confirmers { get; init; } = new();
  //public ReversibleCommandStack UndoStack { get; init; } = new();
  //public Notifier Notifier { get; init; } = new();
  //public Worker Worker { get; init; } = new();

  //[NotNull] public IAsyncRelayCommand<DeckEditorMTGCard>? ChangeCommanderCommand => field ??= new ChangeCommander(this).Command;
  //[NotNull] public IRelayCommand<DeckEditorMTGCard>? RemoveCommanderCommand => field ??= new RemoveCommander(this).Command;
  //[NotNull] public IRelayCommand<DeckEditorMTGCard>? BeginMoveFromCommand => field ??= new MoveCard.BeginMoveFrom(this).Command;
  //[NotNull] public IAsyncRelayCommand<DeckEditorMTGCard>? BeginMoveToCommand => field ??= new MoveCard.BeginMoveTo(this).Command;
  //[NotNull] public IRelayCommand? ExecuteMoveCommand => field ??= new MoveCard.ExecuteMove(this).Command;
  //[NotNull] public IAsyncRelayCommand<string>? ImportCommanderCommand => field ??= new ImportCommander(this).Command;
}

