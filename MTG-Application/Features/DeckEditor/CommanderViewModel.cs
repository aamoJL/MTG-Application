using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MTGApplication.General.Models.Card;
using MTGApplication.General.Services.API.CardAPI;
using MTGApplication.General.Services.ReversibleCommandService;
using MTGApplication.General.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace MTGApplication.Features.DeckEditor;

public partial class CommanderViewModel : ViewModelBase
{
  public CommanderViewModel(ICardAPI<MTGCard> cardAPI)
  {
    CardAPI = cardAPI;

    ReversibleChange ??= new()
    {
      Action = (card) => { Card = card; },
      ReverseAction = (card) => { Card = card; }
    };
  }

  [ObservableProperty] private MTGCard card;

  public ReversibleAction<MTGCard> ReversibleChange { get; init; }
  public ReversibleCommandStack UndoStack { get; init; } = new();
  public IWorker Worker { get; init; } = new DefaultWorker();

  private ICardAPI<MTGCard> CardAPI { get; }
  private MTGCardCopier CardCopier { get; } = new();

  [RelayCommand]
  private void Change(MTGCard card) => UndoStack.PushAndExecute(
    new ReversibleCommanderChangeCommand(card, Card, CardCopier) { ReversibleAction = ReversibleChange });

  [RelayCommand]
  private void Remove(MTGCard card) => UndoStack.PushAndExecute(
    new ReversibleCommanderChangeCommand(null, Card, CardCopier) { ReversibleAction = ReversibleChange });

  [RelayCommand]
  private void BeginMoveFrom(MTGCard card) => UndoStack.ActiveCombinedCommand.Commands.Add(
    new ReversibleCommanderChangeCommand(null, Card, CardCopier) { ReversibleAction = ReversibleChange });

  [RelayCommand]
  private void BeginMoveTo(MTGCard card) => UndoStack.ActiveCombinedCommand.Commands.Add(
    new ReversibleCommanderChangeCommand(card, Card, CardCopier) { ReversibleAction = ReversibleChange });

  [RelayCommand]
  private void ExecuteMove(MTGCard card) => UndoStack.PushAndExecuteActiveCombinedCommand();

  [RelayCommand]
  private async Task Import(string data)
  {
    var result = await Worker.DoWork(new ImportCards(CardAPI).Execute(data));

    if (result.Found.Any())
      Change(result.Found[0]);
  }
}

