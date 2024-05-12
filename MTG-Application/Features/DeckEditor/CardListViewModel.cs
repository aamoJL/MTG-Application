using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MTGApplication.General.Models.Card;
using MTGApplication.General.Services.API.CardAPI;
using MTGApplication.General.Services.ReversibleCommandService;
using MTGApplication.General.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace MTGApplication.Features.DeckEditor;

public partial class CardListViewModel : ObservableObject
{
  public CardListViewModel(ICardAPI<MTGCard> cardAPI) => CardImporter = new(cardAPI);

  [ObservableProperty] private ObservableCollection<MTGCard> cards = new();

  private CardImporter CardImporter { get; }
  private MTGCardCopier CardCopier { get; } = new();

  public ReversibleCommandStack UndoStack { get; init; } = new();
  public IWorker Worker { get; init; } = new DefaultWorker();

  public Action OnChange { get; init; }

  private ReversibleAction<IEnumerable<MTGCard>> ReversableAdd => new() { Action = UndoableAdd, ReverseAction = UndoableRemove };
  private ReversibleAction<IEnumerable<MTGCard>> ReversableRemove => new() { Action = UndoableRemove, ReverseAction = UndoableAdd };

  [RelayCommand]
  private void AddCard(MTGCard card) => UndoStack.PushAndExecute(
    new ReversibleCollectionCommand<MTGCard>(card, CardCopier) { ReversableAction = ReversableAdd });

  [RelayCommand]
  private void RemoveCard(MTGCard card) => UndoStack.PushAndExecute(
    new ReversibleCollectionCommand<MTGCard>(card, CardCopier) { ReversableAction = ReversableRemove });

  [RelayCommand]
  private async Task ImportCards(string data)
  {
    var result = await Worker.DoWork(CardImporter.Import(data));

    if (result.Found.Length > 0)
    {
      UndoStack.PushAndExecute(new ReversibleCollectionCommand<MTGCard>(result.Found, CardCopier)
      {
        ReversableAction = ReversableAdd,
      });
    }
  }

  [RelayCommand]
  private void BeginMoveFrom(MTGCard card) => UndoStack.ActiveCombinedCommand.Commands.Add(
    new ReversibleCollectionCommand<MTGCard>(card, CardCopier) { ReversableAction = ReversableRemove });

  [RelayCommand]
  private void BeginMoveTo(MTGCard card) => UndoStack.ActiveCombinedCommand.Commands.Add(
    new ReversibleCollectionCommand<MTGCard>(card, CardCopier) { ReversableAction = ReversableAdd });

  [RelayCommand]
  private void ExecuteMove(MTGCard card) => UndoStack.PushAndExecuteActiveCombinedCommand();

  [RelayCommand] private void CardlistCardChanged() => OnChange?.Invoke();

  private void UndoableAdd(IEnumerable<MTGCard> cards)
  {
    foreach (var card in cards)
    {
      if (Cards.FirstOrDefault(x => x.Info.Name == card?.Info.Name) is MTGCard existingCard)
        existingCard.Count += card.Count;
      else if (card != null)
        Cards.Add(card);
    }

    OnChange?.Invoke();
  }

  private void UndoableRemove(IEnumerable<MTGCard> cards)
  {
    foreach (var card in cards)
    {
      if (Cards.FirstOrDefault(x => x.Info.Name == card?.Info.Name) is MTGCard existingCard)
      {
        if (existingCard.Count <= card.Count) Cards.Remove(existingCard);
        else existingCard.Count -= card.Count;
      }
    }

    OnChange?.Invoke();
  }
}
