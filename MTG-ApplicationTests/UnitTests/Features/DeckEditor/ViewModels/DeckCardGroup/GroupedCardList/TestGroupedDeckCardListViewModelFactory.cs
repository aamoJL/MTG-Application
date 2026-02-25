using MTGApplication.Features.DeckEditor.Models;
using MTGApplication.Features.DeckEditor.ViewModels.DeckCardGroup.GroupedCardList;
using MTGApplication.General.Services.ReversibleCommandService;
using MTGApplication.General.ViewModels;
using MTGApplicationTests.TestUtility.Exporters;
using MTGApplicationTests.TestUtility.Importers;
using MTGApplicationTests.TestUtility.Services;
using System.Collections.ObjectModel;
using static MTGApplication.Features.DeckEditor.ViewModels.DeckCardList.DeckCardListViewModel;

namespace MTGApplicationTests.UnitTests.Features.DeckEditor.ViewModels.DeckCardGroup.GroupedCardList;

public class TestGroupedDeckCardListViewModelFactory
{
  public ObservableCollection<DeckEditorMTGCard> Model { get; set; } = [];
  public Worker Worker { get; init; } = new();
  public TestMTGCardImporter Importer { get; init; } = new();
  public TestEdhrecImporter EdhrecImporter { get; init; } = new();
  public TestScryfallImporter ScryfallImporter { get; init; } = new();
  public TestStringExporter Exporter { get; init; } = new();
  public ReversibleCommandStack UndoStack { get; init; } = new();
  public TestNotifier Notifier { get; init; } = new NotImplementedNotifier();
  public TestNetworkService NetworkService { get; init; } = new();
  public CardListConfirmers Confirmers { get; init; } = new();
  public GroupedDeckCardListViewModel.GroupedCardListConfirmers GroupedListConfirmers { get; set; } = new();

  public GroupedDeckCardListViewModel Build()
  {
    return new(Model)
    {
      Worker = Worker,
      Importer = Importer,
      EdhrecImporter = EdhrecImporter,
      ScryfallImporter = ScryfallImporter,
      Exporter = Exporter,
      UndoStack = UndoStack,
      Notifier = Notifier,
      NetworkService = NetworkService,
      Confirmers = Confirmers,
      GroupedListConfirmers = GroupedListConfirmers
    };
  }
}
