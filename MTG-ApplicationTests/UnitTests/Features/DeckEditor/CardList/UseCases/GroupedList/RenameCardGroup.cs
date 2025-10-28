using MTGApplication.Features.DeckEditor.ViewModels;
using MTGApplication.General.Services.ConfirmationService;
using MTGApplicationTests.TestUtility.Importers;
using MTGApplicationTests.TestUtility.Mocker;
using MTGApplicationTests.TestUtility.Services;

namespace MTGApplicationTests.UnitTests.Features.DeckEditor.CardList.UseCases.GroupedList;

[TestClass]
public class RenameCardGroup
{
  [TestMethod]
  public void Rename_HasKey_CanExecute()
  {
    var key = "key";
    var viewmodel = new GroupedCardListViewModel(
      cards: [DeckEditorMTGCardMocker.CreateMTGCardModel(name: "B", group: key)],
      importer: new TestMTGCardImporter());

    Assert.IsTrue(viewmodel.RenameGroupCommand.CanExecute(
      viewmodel.Groups.First(x => x.Key == key)));
  }

  [TestMethod]
  public void Rename_EmptyKey_CanNotExecute()
  {
    var key = string.Empty;
    var viewmodel = new GroupedCardListViewModel(
      cards: [DeckEditorMTGCardMocker.CreateMTGCardModel(name: "B", group: key)],
      importer: new TestMTGCardImporter());

    Assert.IsFalse(viewmodel.RenameGroupCommand.CanExecute(
      viewmodel.Groups.First(x => x.Key == key)));
  }

  [TestMethod]
  public async Task Rename_ConfirmationShown()
  {
    var confirmer = new TestConfirmer<string, string>();
    var viewmodel = new GroupedCardListViewModel(
      cards: [DeckEditorMTGCardMocker.CreateMTGCardModel(name: "B", group: "key")],
      importer: new TestMTGCardImporter())
    {
      Confirmers = new()
      {
        RenameCardGroupConfirmer = confirmer
      }
    };

    await viewmodel.RenameGroupCommand.ExecuteAsync(viewmodel.Groups.First(x => x.Key != string.Empty));

    ConfirmationAssert.ConfirmationShown(confirmer);
  }

  [TestMethod]
  public async Task Rename_Conflict_MergeConfirmationShown()
  {
    var oldValue = "old";
    var newValue = "new";

    var confirmer = new TestConfirmer<ConfirmationResult>();
    var viewmodel = new GroupedCardListViewModel(
      cards: [
        DeckEditorMTGCardMocker.CreateMTGCardModel(name: "1", group: oldValue ),
          DeckEditorMTGCardMocker.CreateMTGCardModel(name: "2", group: newValue ),
      ],
      importer: new TestMTGCardImporter())
    {
      Confirmers = new()
      {
        RenameCardGroupConfirmer = new() { OnConfirm = async _ => await Task.FromResult(newValue) },
        MergeCardGroupsConfirmer = confirmer
      }
    };

    await viewmodel.RenameGroupCommand.ExecuteAsync(viewmodel.Groups.First(x => x.Key == oldValue));

    ConfirmationAssert.ConfirmationShown(confirmer);
  }

  [TestMethod]
  public async Task Rename_KeyChanged()
  {
    var oldValue = "old";
    var newValue = "new";

    var viewmodel = new GroupedCardListViewModel(
      cards: [DeckEditorMTGCardMocker.CreateMTGCardModel(name: "B", group: oldValue)],
      importer: new TestMTGCardImporter())
    {
      Confirmers = new()
      {
        RenameCardGroupConfirmer = new() { OnConfirm = async _ => await Task.FromResult(newValue) },
      }
    };

    var group = viewmodel.Groups.First(x => x.Key == oldValue);

    await viewmodel.RenameGroupCommand.ExecuteAsync(group);

    Assert.AreEqual(newValue, group.Key);

    viewmodel.UndoStack.Undo();

    Assert.AreEqual(oldValue, group.Key);

    viewmodel.UndoStack.Redo();

    Assert.AreEqual(newValue, group.Key);
  }

  [TestMethod]
  public async Task Rename_GroupCardsGroupChanged()
  {
    var oldValue = "old";
    var newValue = "new";

    var viewmodel = new GroupedCardListViewModel(
      cards: [
        DeckEditorMTGCardMocker.CreateMTGCardModel(name: "1", group: oldValue ),
          DeckEditorMTGCardMocker.CreateMTGCardModel(name: "2", group: oldValue ),
          DeckEditorMTGCardMocker.CreateMTGCardModel(name: "3", group: oldValue ),
          DeckEditorMTGCardMocker.CreateMTGCardModel(name: "4", group: string.Empty ),
          DeckEditorMTGCardMocker.CreateMTGCardModel(name: "5", group: string.Empty ),
      ],
      importer: new TestMTGCardImporter())
    {
      Confirmers = new()
      {
        RenameCardGroupConfirmer = new() { OnConfirm = async _ => await Task.FromResult(newValue) },
      }
    };

    var group = viewmodel.Groups.First(x => x.Key == oldValue);
    var cardCount = viewmodel.Cards.Count(x => x.Group == oldValue);

    await viewmodel.RenameGroupCommand.ExecuteAsync(group);

    Assert.AreEqual(cardCount, viewmodel.Cards.Count(x => x.Group == newValue));

    viewmodel.UndoStack.Undo();

    Assert.AreEqual(cardCount, viewmodel.Cards.Count(x => x.Group == oldValue));

    viewmodel.UndoStack.Redo();

    Assert.AreEqual(cardCount, viewmodel.Cards.Count(x => x.Group == newValue));
  }

  [TestMethod]
  public async Task Rename_NewName_GroupCountIsSame()
  {
    var oldValue = "old";
    var newValue = "new";

    var viewmodel = new GroupedCardListViewModel(
      cards: [
        DeckEditorMTGCardMocker.CreateMTGCardModel(name: "1", group: oldValue ),
          DeckEditorMTGCardMocker.CreateMTGCardModel(name: "2", group: oldValue ),
          DeckEditorMTGCardMocker.CreateMTGCardModel(name: "3", group: oldValue ),
          DeckEditorMTGCardMocker.CreateMTGCardModel(name: "4", group: string.Empty ),
          DeckEditorMTGCardMocker.CreateMTGCardModel(name: "5", group: string.Empty ),
      ],
      importer: new TestMTGCardImporter())
    {
      Confirmers = new()
      {
        RenameCardGroupConfirmer = new() { OnConfirm = async _ => await Task.FromResult(newValue) },
      }
    };

    var group = viewmodel.Groups.First(x => x.Key == oldValue);
    var groupCount = viewmodel.Groups.Count;

    await viewmodel.RenameGroupCommand.ExecuteAsync(group);

    Assert.HasCount(groupCount, viewmodel.Groups);

    viewmodel.UndoStack.Undo();

    Assert.HasCount(groupCount, viewmodel.Groups);

    viewmodel.UndoStack.Redo();

    Assert.HasCount(groupCount, viewmodel.Groups);
  }

  [TestMethod]
  public async Task Rename_ExistingName_Merge_GroupsMerged()
  {
    var oldValue = "old";
    var newValue = "new";

    var viewmodel = new GroupedCardListViewModel(
      cards: [
        DeckEditorMTGCardMocker.CreateMTGCardModel(name: "1", group: oldValue ),
          DeckEditorMTGCardMocker.CreateMTGCardModel(name: "2", group: oldValue ),
          DeckEditorMTGCardMocker.CreateMTGCardModel(name: "3", group: oldValue ),
          DeckEditorMTGCardMocker.CreateMTGCardModel(name: "4", group: newValue ),
          DeckEditorMTGCardMocker.CreateMTGCardModel(name: "5", group: newValue ),
      ],
      importer: new TestMTGCardImporter())
    {
      Confirmers = new()
      {
        RenameCardGroupConfirmer = new() { OnConfirm = async _ => await Task.FromResult(newValue) },
        MergeCardGroupsConfirmer = new() { OnConfirm = async _ => await Task.FromResult(ConfirmationResult.Yes) }
      }
    };

    var oldGroup = viewmodel.Groups.First(x => x.Key == oldValue);
    var oldCardCount = oldGroup.Count;
    var oldGroupCount = viewmodel.Groups.Count;

    var newGroup = viewmodel.Groups.First(x => x.Key == newValue);
    var newCardCount = viewmodel.Cards.Count;
    var newGroupCount = oldCardCount - 1;

    await viewmodel.RenameGroupCommand.ExecuteAsync(oldGroup);

    Assert.HasCount(newGroupCount, viewmodel.Groups);
    Assert.AreEqual(newCardCount, newGroup.Count);
    Assert.AreEqual(0, oldGroup.Count);

    viewmodel.UndoStack.Undo();

    Assert.HasCount(oldGroupCount, viewmodel.Groups);
    Assert.AreEqual(oldCardCount, oldGroup.Count);
    Assert.AreEqual(newCardCount - oldCardCount, newGroup.Count);

    viewmodel.UndoStack.Redo();

    Assert.HasCount(newGroupCount, viewmodel.Groups);
    Assert.AreEqual(newCardCount, newGroup.Count);
    Assert.AreEqual(0, oldGroup.Count);
  }
}
