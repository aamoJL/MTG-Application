using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MTGApplication.Features.DeckEditor.Services.Cardlist;
using MTGApplication.Features.DeckEditor.Services.Commanders;
using MTGApplication.General.Models.Card;
using MTGApplication.General.Services.API.CardAPI;
using MTGApplication.General.Services.ReversibleCommandService;
using MTGApplication.General.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplication.Features.DeckEditor;

public partial class CommanderViewModel : ViewModelBase
{
  public CommanderViewModel(MTGCardImporter importer)
  {
    Importer = importer;

    ReversibleChange ??= new()
    {
      Action = (card) => { Card = card; },
      ReverseAction = (card) => { Card = card; }
    };
  }

  [ObservableProperty] private DeckEditorMTGCard card;

  public ReversibleAction<DeckEditorMTGCard> ReversibleChange { get; set; }
  public Action OnCardPropertyChange { get; init; }

  public CommanderConfirmers Confirmers { get; init; } = new();
  public ReversibleCommandStack UndoStack { get; init; } = new();
  public IWorker Worker { get; init; } = new DefaultWorker();
  public Notifier Notifier { get; init; } = new();

  private MTGCardImporter Importer { get; }
  private DeckEditorMTGCardCopier CardCopier { get; } = new();

  private ReversibleAction<(DeckEditorMTGCard Card, DeckEditorMTGCard.MTGCardInfo Info)> ReversibleCardPrintChangeAction
    => new() { Action = (arg) => ReversibleCardPrintChange(arg.Card, arg.Info), ReverseAction = (arg) => ReversibleCardPrintChange(arg.Card, arg.Info) };

  [RelayCommand]
  private async Task Change(DeckEditorMTGCard card)
  {
    UndoStack.PushAndExecute(new ReversibleCommanderChangeCommand(card, Card, CardCopier) { ReversibleAction = ReversibleChange });
    await Task.Yield();
  }

  [RelayCommand(CanExecute = nameof(CanExecuteRemoveCommand))]
  private void Remove(DeckEditorMTGCard card) => UndoStack.PushAndExecute(
    new ReversibleCommanderChangeCommand(null, Card, CardCopier) { ReversibleAction = ReversibleChange });

  [RelayCommand]
  private void BeginMoveFrom(DeckEditorMTGCard card) => UndoStack.ActiveCombinedCommand.Commands.Add(
    new ReversibleCommanderChangeCommand(null, Card, CardCopier) { ReversibleAction = ReversibleChange });

  [RelayCommand]
  private async Task BeginMoveTo(DeckEditorMTGCard card)
  {
    UndoStack.ActiveCombinedCommand.Commands.Add(new ReversibleCommanderChangeCommand(card, Card, CardCopier) { ReversibleAction = ReversibleChange });
    await Task.Yield();
  }

  [RelayCommand]
  private void ExecuteMove(DeckEditorMTGCard card) => UndoStack.PushAndExecuteActiveCombinedCommand();

  [RelayCommand]
  private async Task Import(string data)
  {
    var result = await Worker.DoWork(new ImportCards(Importer).Execute(data));

    if (result.Found.Length == 0)
    {
      new SendNotification(Notifier).Execute(CommanderNotifications.ImportError);
    }
    else if (!result.Found[0].Info.TypeLine.Contains("Legendary", System.StringComparison.OrdinalIgnoreCase))
    {
      new SendNotification(Notifier).Execute(CommanderNotifications.ImportNotLegendaryError);
    }
    else
    {
      // Only legendary cards are allowed to be commanders
      await Change(result.Found[0]);

      new SendNotification(Notifier).Execute(CommanderNotifications.ImportSuccess);
    }
  }

  [RelayCommand(CanExecute = nameof(CanExecuteChangeCardPrintCommand))]
  private async Task ChangeCardPrint(DeckEditorMTGCard card)
  {
    if (Card.Info.Name != card.Info.Name) return;

    var prints = (await Worker.DoWork(Importer.ImportFromUri(pageUri: Card.Info.PrintSearchUri, paperOnly: true, fetchAll: true))).Found;

    if (await Confirmers.ChangeCardPrintConfirmer.Confirm(CardListConfirmers.GetChangeCardPrintConfirmation(prints)) is DeckEditorMTGCard selection)
    {
      if (selection.Info.ScryfallId == Card.Info.ScryfallId)
        return; // Same print

      UndoStack.PushAndExecute(
        new ReversiblePropertyChangeCommand<DeckEditorMTGCard, DeckEditorMTGCard.MTGCardInfo>(Card, Card.Info, selection.Info, CardCopier)
        {
          ReversibleAction = ReversibleCardPrintChangeAction
        });
    }
  }

  private bool CanExecuteRemoveCommand() => Card != null;

  private bool CanExecuteChangeCardPrintCommand() => Card != null;

  private void ReversibleCardPrintChange(DeckEditorMTGCard card, DeckEditorMTGCard.MTGCardInfo info)
  {
    if (Card.Info.Name != card.Info.Name) return;

    Card.Info = info with { };
    OnCardPropertyChange?.Invoke();
  }
}

