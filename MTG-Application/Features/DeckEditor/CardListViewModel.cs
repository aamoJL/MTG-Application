using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MTGApplication.General.Models.Card;
using MTGApplication.General.Services.API.CardAPI;
using MTGApplication.General.Services.ConfirmationService;
using MTGApplication.General.Services.IOService;
using MTGApplication.General.Services.NotificationService;
using MTGApplication.General.Services.ReversibleCommandService;
using MTGApplication.General.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplication.Features.DeckEditor;

public partial class CardListViewModel(ICardAPI<MTGCard> cardAPI) : ViewModelBase
{
  [ObservableProperty] private ObservableCollection<MTGCard> cards = [];

  public ICardAPI<MTGCard> CardAPI { get; } = cardAPI;

  public ReversibleCommandStack UndoStack { get; init; } = new();
  public CardListConfirmers Confirmers { get; init; } = new();
  public ClipboardService ClipboardService { get; init; } = new();
  public Notifier Notifier { get; init; } = new();
  public IWorker Worker { get; init; } = IWorker.Default;

  public Action OnChange { get; init; }

  private MTGCardCopier CardCopier { get; } = new();
  private ReversibleAction<IEnumerable<MTGCard>> ReversibleAddAction => new() { Action = ReversibleAdd, ReverseAction = ReversibleRemove };
  private ReversibleAction<IEnumerable<MTGCard>> ReversibleRemoveAction => new() { Action = ReversibleRemove, ReverseAction = ReversibleAdd };
  private ReversibleAction<(MTGCard Card, int Value)> ReversibleCardCountChangeAction
    => new() { Action = (arg) => ReversibleCardCountChange(arg.Card, arg.Value), ReverseAction = (arg) => ReversibleCardCountChange(arg.Card, arg.Value) };
  private ReversibleAction<(MTGCard Card, MTGCard.MTGCardInfo Info)> ReversibleCardPrintChangeAction
    => new() { Action = (arg) => ReversibleCardPrintChange(arg.Card, arg.Info), ReverseAction = (arg) => ReversibleCardPrintChange(arg.Card, arg.Info) };

  [RelayCommand]
  private async Task AddCard(MTGCard card)
  {
    if (Cards.FirstOrDefault(x => x.Info.Name == card.Info.Name) != null)
    {
      if (await Confirmers.AddSingleConflictConfirmer.Confirm(CardListConfirmers.GetAddSingleConflictConfirmation(card.Info.Name)) is ConfirmationResult.Yes)
        UndoStack.PushAndExecute(new ReversibleCollectionCommand<MTGCard>(card, CardCopier) { ReversibleAction = ReversibleAddAction });
    }
    else
      UndoStack.PushAndExecute(new ReversibleCollectionCommand<MTGCard>(card, CardCopier) { ReversibleAction = ReversibleAddAction });
  }

  [RelayCommand]
  private void RemoveCard(MTGCard card) => UndoStack.PushAndExecute(
    new ReversibleCollectionCommand<MTGCard>(card, CardCopier) { ReversibleAction = ReversibleRemoveAction });

  [RelayCommand]
  private void BeginMoveFrom(MTGCard card) => UndoStack.ActiveCombinedCommand.Commands.Add(
    new ReversibleCollectionCommand<MTGCard>(card, CardCopier) { ReversibleAction = ReversibleRemoveAction });

  [RelayCommand]
  private async Task BeginMoveTo(MTGCard card)
  {
    if (Cards.FirstOrDefault(x => x.Info.Name == card.Info.Name) != null)
    {
      if (await Confirmers.AddSingleConflictConfirmer.Confirm(CardListConfirmers.GetAddSingleConflictConfirmation(card.Info.Name)) is ConfirmationResult.Yes)
        UndoStack.ActiveCombinedCommand.Commands.Add(new ReversibleCollectionCommand<MTGCard>(card, CardCopier) { ReversibleAction = ReversibleAddAction });
      else
        UndoStack.ActiveCombinedCommand.Cancel();
    }
    else
      UndoStack.ActiveCombinedCommand.Commands.Add(new ReversibleCollectionCommand<MTGCard>(card, CardCopier) { ReversibleAction = ReversibleAddAction });
  }

  [RelayCommand]
  private void ExecuteMove(MTGCard card) => UndoStack.PushAndExecuteActiveCombinedCommand();

  [RelayCommand(CanExecute = nameof(CanExecuteClearCommand))]
  private void Clear() => UndoStack.PushAndExecute(
    new ReversibleCollectionCommand<MTGCard>(Cards, CardCopier) { ReversibleAction = ReversibleRemoveAction });

  [RelayCommand]
  private async Task ImportCards(string data)
  {
    data ??= await Confirmers.ImportConfirmer.Confirm(CardListConfirmers.GetImportConfirmation(string.Empty));

    var result = await Worker.DoWork(new ImportCards(CardAPI).Execute(data));

    var addedCards = new List<MTGCard>();
    var skipConflictConfirmation = false;
    var addConflictConfirmationResult = ConfirmationResult.Yes;

    // Confirm imported cards, if already exists
    foreach (var card in result.Found)
    {
      if (Cards.FirstOrDefault(x => x.Info.Name == card.Info.Name) is not null)
      {
        // Card exist in the list; confirm the import, unless the user skips confirmations
        if (!skipConflictConfirmation)
        {
          (addConflictConfirmationResult, skipConflictConfirmation) = await Confirmers.AddMultipleConflictConfirmer.Confirm(
            CardListConfirmers.GetAddMultipleConflictConfirmation(card.Info.Name));
        }

        if (addConflictConfirmationResult == ConfirmationResult.Yes) { addedCards.Add(card); }
      }
      else if (addedCards.FirstOrDefault(x => x.Info.Name == card.Info.Name) is MTGCard addedCard)
        addedCard.Count += card.Count;
      else
        addedCards.Add(card);
    }

    // Add cards
    if (addedCards.Count != 0)
    {
      UndoStack.PushAndExecute(new ReversibleCollectionCommand<MTGCard>(addedCards, CardCopier)
      {
        ReversibleAction = ReversibleAddAction,
      });
    }

    var skippedCount = result.Found.Length - addedCards.Count;

    // Notifications
    if (result.Source == CardImportResult.ImportSource.External)
    {
      if (result.Found.Length != 0 && result.NotFoundCount == 0) // All found
        new SendNotification(Notifier).Execute(new(NotificationType.Success,
          $"{addedCards.Count} cards imported successfully." + (skippedCount > 0 ? $" ({skippedCount} cards skipped) " : "")));
      else if (result.Found.Length != 0 && result.NotFoundCount > 0) // Some found
        new SendNotification(Notifier).Execute(new(NotificationType.Warning,
          $"{addedCards.Count} / {result.NotFoundCount + addedCards.Count} cards imported successfully.{Environment.NewLine}{result.NotFoundCount} cards were not found." + (skippedCount > 0 ? $" ({skippedCount} cards skipped) " : "")));
      else if (result.NotFoundCount > 0) // None found
        new SendNotification(Notifier).Execute(new(NotificationType.Error, $"Error. No cards were imported."));
    }
  }

  [RelayCommand(CanExecute = nameof(CanExecuteExportCommand))]
  private async Task Export(string byProperty)
  {
    if (string.IsNullOrEmpty(byProperty)) return;

    var exportString = new ExportCards().Execute(new(Cards, byProperty));

    if (await Confirmers.ExportConfirmer.Confirm(CardListConfirmers.GetExportConfirmation(exportString))
      is string response && !string.IsNullOrEmpty(response))
    {
      ClipboardService.CopyToClipboard(response);
      new SendNotification(Notifier).Execute(ClipboardService.CopiedNotification);
    }
  }

  [RelayCommand(CanExecute = nameof(CanExecuteChangeCardCount))]
  private void ChangeCardCount(CardCountChangeArgs args)
  {
    if (!CanExecuteChangeCardCount(args)) return;

    var (card, newValue) = args;

    UndoStack.PushAndExecute(
      new ReversiblePropertyChangeCommand<MTGCard, int>(card, card.Count, newValue, CardCopier) { ReversibleAction = ReversibleCardCountChangeAction });
  }

  [RelayCommand]
  private async Task ChangeCardPrint(MTGCard card)
  {
    if (Cards.FirstOrDefault(x => x.Info.Name == card.Info.Name) is MTGCard existingCard)
    {
      var prints = (await Worker.DoWork(CardAPI.FetchFromUri(pageUri: existingCard.Info.PrintSearchUri, paperOnly: true, fetchAll: true))).Found;

      if (await Confirmers.ChangeCardPrintConfirmer.Confirm(CardListConfirmers.GetChangeCardPrintConfirmation(prints)) is MTGCard selection)
      {
        if (selection.Info.ScryfallId == existingCard.Info.ScryfallId)
          return; // Same print

        UndoStack.PushAndExecute(
          new ReversiblePropertyChangeCommand<MTGCard, MTGCard.MTGCardInfo>(existingCard, existingCard.Info, selection.Info, CardCopier)
          {
            ReversibleAction = ReversibleCardPrintChangeAction
          });
      }
    }
  }

  private bool CanExecuteClearCommand() => Cards.Any();

  private static bool CanExecuteExportCommand(string byProperty) => byProperty is "Id" or "Name";

  private static bool CanExecuteChangeCardCount(CardCountChangeArgs args)
    => args.Card.Count != args.NewValue && args.NewValue > 0;

  private void ReversibleAdd(IEnumerable<MTGCard> cards)
  {
    var addList = new List<MTGCard>();

    foreach (var card in cards)
    {
      if (Cards.FirstOrDefault(x => x.Info.Name == card?.Info.Name) is MTGCard existingCard)
        existingCard.Count += card.Count;
      else if (card != null)
        addList.Add(card);
    }

    foreach (var item in addList)
      Cards.Add(item);

    OnChange?.Invoke();
  }

  private void ReversibleRemove(IEnumerable<MTGCard> cards)
  {
    var removeList = new List<MTGCard>();

    foreach (var card in cards)
    {
      if (Cards.FirstOrDefault(x => x.Info.Name == card?.Info.Name) is MTGCard existingCard)
      {
        if (existingCard.Count <= card.Count) removeList.Add(existingCard);
        else existingCard.Count -= card.Count;
      }
    }

    foreach (var item in removeList)
      Cards.Remove(item);

    OnChange?.Invoke();
  }

  private void ReversibleCardCountChange(MTGCard card, int value)
  {
    if (Cards.FirstOrDefault(x => x.Info.Name == card.Info.Name) is MTGCard existingCard)
    {
      existingCard.Count = value;
      OnChange?.Invoke();
    }
  }

  private void ReversibleCardPrintChange(MTGCard card, MTGCard.MTGCardInfo info)
  {
    if (Cards.FirstOrDefault(x => x.Info.Name == card.Info.Name) is MTGCard existingCard)
    {
      existingCard.Info = info with { };
      OnChange?.Invoke();
    }
  }
}