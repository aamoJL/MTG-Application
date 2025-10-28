using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplication.Features.DeckEditor.ViewModels;
using MTGApplication.General.Services.ConfirmationService;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplicationTests.TestUtility.Importers;
using MTGApplicationTests.TestUtility.Mocker;
using MTGApplicationTests.TestUtility.Services;
using System.Collections.ObjectModel;

namespace MTGApplicationTests.UnitTests.Features.DeckEditor.CardList.UseCases.GroupedList;

[TestClass]
public class ImportCardsToGroup
{
  [TestMethod]
  public async Task Import_ConfirmationShown()
  {
    var confirmer = new TestConfirmer<string, string>();
    var viewmodel = new CardGroupViewModel(string.Empty, [], new TestMTGCardImporter())
    {
      Confirmers = new()
      {
        ImportConfirmer = confirmer
      }
    };

    await viewmodel.ImportCardsCommand.ExecuteAsync(null);

    ConfirmationAssert.ConfirmationShown(confirmer);
  }

  [TestMethod]
  public async Task Import_NewCards_CardsAdded()
  {
    var source = new ObservableCollection<DeckEditorMTGCard>();
    var cards = new CardImportResult.Card[]
    {
        new(MTGCardInfoMocker.MockInfo(name: "1")),
        new(MTGCardInfoMocker.MockInfo(name: "2")),
        new(MTGCardInfoMocker.MockInfo(name: "3")),
    };
    var viewmodel = new CardGroupViewModel("Key", source, new TestMTGCardImporter()
    {
      ExpectedCards = cards
    })
    {
      Confirmers = new()
      {
        ImportConfirmer = new()
        {
          OnConfirm = async _ => await Task.FromResult("data")
        }
      }
    };

    await viewmodel.ImportCardsCommand.ExecuteAsync(null);

    CollectionAssert.AreEquivalent(
      cards.Select(x => x.Info).ToArray(),
      viewmodel.Cards.Select(x => x.Info).ToArray());

    viewmodel.UndoStack.Undo();

    Assert.IsEmpty(viewmodel.Source);

    viewmodel.UndoStack.Redo();

    CollectionAssert.AreEquivalent(
      cards.Select(x => x.Info).ToArray(),
      viewmodel.Cards.Select(x => x.Info).ToArray());
  }

  [TestMethod]
  public async Task Import_ExistingCards_ConfirmationShown()
  {
    var confirmer = new TestConfirmer<(ConfirmationResult, bool)>();
    var cards = new CardImportResult.Card[]
    {
        new(MTGCardInfoMocker.MockInfo(name: "1")),
        new(MTGCardInfoMocker.MockInfo(name: "2")),
        new(MTGCardInfoMocker.MockInfo(name: "3")),
    };
    var source = new ObservableCollection<DeckEditorMTGCard>(cards.Select(x => new DeckEditorMTGCard(x.Info)));
    var viewmodel = new CardGroupViewModel("Key", source, new TestMTGCardImporter()
    {
      ExpectedCards = cards
    })
    {
      Confirmers = new()
      {
        ImportConfirmer = new() { OnConfirm = async _ => await Task.FromResult("data") },
        AddMultipleConflictConfirmer = confirmer
      }
    };

    await viewmodel.ImportCardsCommand.ExecuteAsync(null);

    ConfirmationAssert.ConfirmationShown(confirmer);
  }

  [TestMethod]
  public async Task Import_ExistingCards_CardGroupsChanged()
  {
    var cards = new CardImportResult.Card[]
    {
        new(MTGCardInfoMocker.MockInfo(name: "1")),
        new(MTGCardInfoMocker.MockInfo(name: "2")),
        new(MTGCardInfoMocker.MockInfo(name: "3")),
    };
    var source = new ObservableCollection<DeckEditorMTGCard>(cards.Select(x => new DeckEditorMTGCard(x.Info)));
    var viewmodel = new CardGroupViewModel("Key", source, new TestMTGCardImporter()
    {
      ExpectedCards = cards
    })
    {
      Confirmers = new()
      {
        ImportConfirmer = new() { OnConfirm = async _ => await Task.FromResult("data") },
        AddMultipleConflictConfirmer = new()
        {
          OnConfirm = async _ => await Task.FromResult((ConfirmationResult.Yes, true))
        }
      }
    };

    await viewmodel.ImportCardsCommand.ExecuteAsync(null);

    Assert.IsTrue(viewmodel.Source.All(x => x.Group == viewmodel.Key));
    Assert.IsTrue(viewmodel.Source.All(x => x.Count == 2));

    viewmodel.UndoStack.Undo();

    Assert.IsEmpty(viewmodel.Cards);
    Assert.IsTrue(viewmodel.Source.All(x => x.Count == 1));
    Assert.IsTrue(viewmodel.Source.All(x => x.Group != viewmodel.Key));

    viewmodel.UndoStack.Redo();

    Assert.IsTrue(viewmodel.Source.All(x => x.Group == viewmodel.Key));
    Assert.IsTrue(viewmodel.Source.All(x => x.Count == 2));
  }
}
