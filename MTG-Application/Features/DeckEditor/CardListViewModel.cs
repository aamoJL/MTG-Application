using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MTGApplication.General.Models.Card;
using MTGApplication.General.Services.API.CardAPI;
using MTGApplication.General.Services.ConfirmationService;
using MTGApplication.General.Services.IOService;
using MTGApplication.General.Services.ReversibleCommandService;
using MTGApplication.General.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplication.Features.DeckEditor;

public partial class CardListViewModel : ViewModelBase
{
  public CardListViewModel(ICardAPI<MTGCard> cardAPI) => CardAPI = cardAPI;

  [ObservableProperty] private ObservableCollection<MTGCard> cards = new();

  private MTGCardCopier CardCopier { get; } = new();

  public ReversibleCommandStack UndoStack { get; init; } = new();
  public CardListConfirmers Confirmers { get; init; } = new();
  public ClipboardService ClipboardService { get; init; } = new();
  public Notifier Notifier { get; init; } = new();
  public IWorker Worker { get; init; } = new DefaultWorker();

  public Action OnChange { get; init; }

  private ReversibleAction<IEnumerable<MTGCard>> ReversableAdd => new() { Action = ReversibleAdd, ReverseAction = ReversibleRemove };
  private ReversibleAction<IEnumerable<MTGCard>> ReversableRemove => new() { Action = ReversibleRemove, ReverseAction = ReversibleAdd };

  public ICardAPI<MTGCard> CardAPI { get; }

  [RelayCommand]
  private void AddCard(MTGCard card) => UndoStack.PushAndExecute(
    new ReversibleCollectionCommand<MTGCard>(card, CardCopier) { ReversibleAction = ReversableAdd });

  [RelayCommand]
  private void RemoveCard(MTGCard card) => UndoStack.PushAndExecute(
    new ReversibleCollectionCommand<MTGCard>(card, CardCopier) { ReversibleAction = ReversableRemove });

  [RelayCommand]
  private void BeginMoveFrom(MTGCard card) => UndoStack.ActiveCombinedCommand.Commands.Add(
    new ReversibleCollectionCommand<MTGCard>(card, CardCopier) { ReversibleAction = ReversableRemove });

  [RelayCommand]
  private void BeginMoveTo(MTGCard card) => UndoStack.ActiveCombinedCommand.Commands.Add(
    new ReversibleCollectionCommand<MTGCard>(card, CardCopier) { ReversibleAction = ReversableAdd });

  [RelayCommand]
  private void ExecuteMove(MTGCard card) => UndoStack.PushAndExecuteActiveCombinedCommand();

  [RelayCommand(CanExecute = nameof(CanExecuteClearCommand))]
  private void Clear() => UndoStack.PushAndExecute(
    new ReversibleCollectionCommand<MTGCard>(Cards, CardCopier) { ReversibleAction = ReversableRemove });

  [RelayCommand]
  private async Task ImportCards(string data)
  {
    data ??= await Confirmers.ImportConfirmer.Confirm(CardListConfirmers.GetImportConfirmation(string.Empty));

    var result = await Worker.DoWork(new ImportCards(CardAPI).Execute(data));

    var addedCards = new List<MTGCard>();
    var skipConflictConfirmation = false;
    var importConflictConfirmationResult = ConfirmationResult.Yes;

    // Confirm imported cards, if already exists
    foreach (var card in result.Found)
    {
      if (Cards.FirstOrDefault(x => x.Info.Name == card.Info.Name) is not null)
      {
        // Card exist in the list; confirm the import, unless the user skips confirmations
        if (!skipConflictConfirmation)
          (importConflictConfirmationResult, skipConflictConfirmation) = await Confirmers.ImportConflictConfirmer.Confirm(
            CardListConfirmers.GetExistingCardImportConfirmer(card.Info.Name));

        if (importConflictConfirmationResult == ConfirmationResult.Yes) { addedCards.Add(card); }
      }
      else { addedCards.Add(card); }
    }

    // Add cards
    if (addedCards.Any())
    {
      UndoStack.PushAndExecute(new ReversibleCollectionCommand<MTGCard>(addedCards, CardCopier)
      {
        ReversibleAction = ReversableAdd,
      });
    }

    // Notifications
    if (result.Source == CardImportResult.ImportSource.External)
    {
      if (result.Found.Any() && result.NotFoundCount == 0) // All found
        new SendNotification(Notifier).Execute(new(NotificationType.Success,
          $"{addedCards.Count} cards imported successfully." + ((result.Found.Length - addedCards.Count) > 0 ? $" ({(result.Found.Length - addedCards.Count)} cards skipped) " : "")));
      else if (result.Found.Any() && result.NotFoundCount > 0) // Some found
        new SendNotification(Notifier).Execute(new(NotificationType.Warning,
          $"{result.Found.Length} / {result.NotFoundCount + result.Found.Length} cards imported successfully.{Environment.NewLine}{result.NotFoundCount} cards were not found." + ((result.Found.Length - addedCards.Count) > 0 ? $" ({(result.Found.Length - addedCards.Count)} cards skipped) " : "")));
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

  [RelayCommand] private void CardlistCardChanged() => OnChange?.Invoke();

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

  private bool CanExecuteClearCommand() => Cards.Any();

  private static bool CanExecuteExportCommand(string byProperty) => byProperty is "Id" or "Name";
}
