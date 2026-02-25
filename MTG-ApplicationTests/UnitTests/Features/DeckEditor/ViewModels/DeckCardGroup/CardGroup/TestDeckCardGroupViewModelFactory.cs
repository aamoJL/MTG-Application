using MTGApplication.Features.DeckEditor.Models;
using MTGApplication.Features.DeckEditor.ViewModels.DeckCard;
using MTGApplication.Features.DeckEditor.ViewModels.DeckCardGroup.CardGroup;
using MTGApplication.General.Services.ReversibleCommandService;
using MTGApplication.General.ViewModels;
using MTGApplicationTests.TestUtility.Importers;
using MTGApplicationTests.TestUtility.Services;
using static MTGApplication.Features.DeckEditor.ViewModels.DeckCardGroup.CardGroup.DeckCardGroupViewModel;
using static MTGApplication.Features.DeckEditor.ViewModels.DeckCardList.DeckCardListViewModel;

namespace MTGApplicationTests.UnitTests.Features.DeckEditor.ViewModels.DeckCardGroup.CardGroup;

public class TestDeckCardGroupViewModelFactory
{
  public DeckEditorCardGroup Model { get; set; } = new(string.Empty, []);
  public Worker Worker { get; init; } = new();
  public TestMTGCardImporter Importer { get; init; } = new();
  public TestEdhrecImporter EdhrecImporter { get; init; } = new();
  public TestScryfallImporter ScryfallImporter { get; init; } = new();
  public ReversibleCommandStack UndoStack { get; init; } = new();
  public TestNotifier Notifier { get; init; } = new NotImplementedNotifier();
  public GroupConfirmers Confirmers { get; init; } = new();
  public CardListConfirmers ListConfirmers { get; init; } = new();
  public DeckCardViewModel.CardConfirmers CardConfirmers { get; set; } = new();
  public TestNetworkService NetworkService { get; set; } = new();
  public Action<DeckEditorMTGCard> OnCardDelete { get; set; } = _ => throw new NotImplementedException("OnCardDelete");
  public Action<DeckEditorCardGroup> OnGroupDelete { get; set; } = _ => throw new NotImplementedException("OnGroupDelete");
  public Action<DeckEditorCardGroup, string> OnGroupRename { get; set; } = (_, _) => throw new NotImplementedException("OnGroupDelete");

  public DeckCardGroupViewModel Build()
  {
    return new(Model)
    {
      Worker = Worker,
      Importer = Importer,
      EdhrecImporter = EdhrecImporter,
      ScryfallImporter = ScryfallImporter,
      UndoStack = UndoStack,
      Notifier = Notifier,
      Confirmers = Confirmers,
      ListConfirmers = ListConfirmers,
      OnDelete = OnGroupDelete,
      OnRename = OnGroupRename,
      CardViewModelFactory = new()
      {
        Worker = Worker,
        UndoStack = UndoStack,
        Notifier = Notifier,
        Importer = Importer,
        NetworkService = NetworkService,
        Confirmers = CardConfirmers,
        OnCardDelete = OnCardDelete
      }
    };
  }
}
