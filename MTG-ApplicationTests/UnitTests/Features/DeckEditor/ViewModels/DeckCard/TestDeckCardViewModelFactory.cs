using MTGApplication.Features.DeckEditor.Models;
using MTGApplication.Features.DeckEditor.ViewModels.DeckCard;
using MTGApplication.General.Services.ReversibleCommandService;
using MTGApplication.General.ViewModels;
using MTGApplicationTests.TestUtility.Importers;
using MTGApplicationTests.TestUtility.Mocker;
using MTGApplicationTests.TestUtility.Services;
using static MTGApplication.Features.DeckEditor.ViewModels.DeckCard.DeckCardViewModel;

namespace MTGApplicationTests.UnitTests.Features.DeckEditor.ViewModels.DeckCard;

public class TestDeckCardViewModelFactory
{
  public DeckEditorMTGCard Model { get; set; } = new(MTGCardInfoMocker.MockInfo());
  public Worker Worker { get; init; } = new();
  public TestMTGCardImporter Importer { get; init; } = new();
  public ReversibleCommandStack UndoStack { get; init; } = new();
  public TestNotifier Notifier { get; init; } = new NotImplementedNotifier();
  public TestNetworkService NetworkService { get; init; } = new();
  public CardConfirmers Confirmers { get; init; } = new();
  public Action<DeckEditorMTGCard> OnCardDelete { get; set; } = _ => throw new NotImplementedException("OnCardDelete");

  public DeckCardViewModel Build()
  {
    return new(Model)
    {
      EditorDependencies = new()
      {
        Worker = Worker,
        Importer = Importer,
        Notifier = Notifier,
        NetworkService = NetworkService,
        CardConfirmers = Confirmers,
      },
      UndoStack = UndoStack,
      OnDelete = OnCardDelete,
    };
  }

  public DeckCardViewModel Build(DeckEditorMTGCard model)
  {
    Model = model;

    return new(model)
    {
      EditorDependencies = new()
      {
        Worker = Worker,
        Importer = Importer,
        Notifier = Notifier,
        NetworkService = NetworkService,
        CardConfirmers = Confirmers,
      },
      UndoStack = UndoStack,
      OnDelete = OnCardDelete,
    };
  }
}
