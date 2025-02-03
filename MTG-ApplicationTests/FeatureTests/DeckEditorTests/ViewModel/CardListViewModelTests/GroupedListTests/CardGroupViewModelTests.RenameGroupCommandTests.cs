using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Features.DeckEditor.ViewModels;
using MTGApplication.General.Services.ConfirmationService;
using MTGApplicationTests.TestUtility.Importers;
using MTGApplicationTests.TestUtility.Mocker;
using MTGApplicationTests.TestUtility.Services;

namespace MTGApplicationTests.FeatureTests.DeckEditorTests.ViewModel.CardListViewModelTests.GroupedListTests;

public partial class GroupedCardListViewModelTests
{
  [TestClass]
  public class RenameGroupCommandTests
  {
    [TestMethod]
    public void Rename_HasKey_CanExecute()
    {
      var key = "key";
      var viewmodel = new GroupedCardListViewModel(
        importer: new TestMTGCardImporter())
      {
        Cards = [
          DeckEditorMTGCardMocker.CreateMTGCardModel(name: "B", group: key),
        ]
      };

      Assert.IsTrue(viewmodel.RenameGroupCommand.CanExecute(
        viewmodel.Groups.First(x => x.Key == key)));
    }

    [TestMethod]
    public void Rename_EmptyKey_CanNotExecute()
    {
      var key = string.Empty;
      var viewmodel = new GroupedCardListViewModel(
        importer: new TestMTGCardImporter())
      {
        Cards = [
          DeckEditorMTGCardMocker.CreateMTGCardModel(name: "B", group: key),
        ]
      };

      Assert.IsFalse(viewmodel.RenameGroupCommand.CanExecute(
        viewmodel.Groups.First(x => x.Key == key)));
    }

    [TestMethod]
    public async Task Rename_ConfirmationShown()
    {
      var confirmer = new TestConfirmer<string, string>();
      var viewmodel = new GroupedCardListViewModel(
        importer: new TestMTGCardImporter(),
        confirmers: new()
        {
          RenameCardGroupConfirmer = confirmer
        })
      {
        Cards = [
          DeckEditorMTGCardMocker.CreateMTGCardModel(name: "B", group: "key"),
        ],
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
        importer: new TestMTGCardImporter(),
        confirmers: new()
        {
          RenameCardGroupConfirmer = new() { OnConfirm = async _ => await Task.FromResult(newValue) },
          MergeCardGroupsConfirmer = confirmer
        })
      {
        Cards = [
          DeckEditorMTGCardMocker.CreateMTGCardModel(name: "1", group: oldValue ),
          DeckEditorMTGCardMocker.CreateMTGCardModel(name: "2", group: newValue ),
        ],
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
        importer: new TestMTGCardImporter(),
        confirmers: new()
        {
          RenameCardGroupConfirmer = new() { OnConfirm = async _ => await Task.FromResult(newValue) },
        })
      {
        Cards = [
          DeckEditorMTGCardMocker.CreateMTGCardModel(name: "B", group: oldValue ),
        ],
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
        importer: new TestMTGCardImporter(),
        confirmers: new()
        {
          RenameCardGroupConfirmer = new() { OnConfirm = async _ => await Task.FromResult(newValue) },
        })
      {
        Cards = [
          DeckEditorMTGCardMocker.CreateMTGCardModel(name: "1", group: oldValue ),
          DeckEditorMTGCardMocker.CreateMTGCardModel(name: "2", group: oldValue ),
          DeckEditorMTGCardMocker.CreateMTGCardModel(name: "3", group: oldValue ),
          DeckEditorMTGCardMocker.CreateMTGCardModel(name: "4", group: string.Empty ),
          DeckEditorMTGCardMocker.CreateMTGCardModel(name: "5", group: string.Empty ),
        ],
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
        importer: new TestMTGCardImporter(),
        confirmers: new()
        {
          RenameCardGroupConfirmer = new() { OnConfirm = async _ => await Task.FromResult(newValue) },
        })
      {
        Cards = [
          DeckEditorMTGCardMocker.CreateMTGCardModel(name: "1", group: oldValue ),
          DeckEditorMTGCardMocker.CreateMTGCardModel(name: "2", group: oldValue ),
          DeckEditorMTGCardMocker.CreateMTGCardModel(name: "3", group: oldValue ),
          DeckEditorMTGCardMocker.CreateMTGCardModel(name: "4", group: string.Empty ),
          DeckEditorMTGCardMocker.CreateMTGCardModel(name: "5", group: string.Empty ),
        ],
      };

      var group = viewmodel.Groups.First(x => x.Key == oldValue);
      var groupCount = viewmodel.Groups.Count;

      await viewmodel.RenameGroupCommand.ExecuteAsync(group);

      Assert.AreEqual(groupCount, viewmodel.Groups.Count);

      viewmodel.UndoStack.Undo();

      Assert.AreEqual(groupCount, viewmodel.Groups.Count);

      viewmodel.UndoStack.Redo();

      Assert.AreEqual(groupCount, viewmodel.Groups.Count);
    }

    [TestMethod]
    public async Task Rename_ExistingName_Merge_GroupsMerged()
    {
      var oldValue = "old";
      var newValue = "new";

      var viewmodel = new GroupedCardListViewModel(
        importer: new TestMTGCardImporter(),
        confirmers: new()
        {
          RenameCardGroupConfirmer = new() { OnConfirm = async _ => await Task.FromResult(newValue) },
          MergeCardGroupsConfirmer = new() { OnConfirm = async _ => await Task.FromResult(ConfirmationResult.Yes) }
        })
      {
        Cards = [
          DeckEditorMTGCardMocker.CreateMTGCardModel(name: "1", group: oldValue ),
          DeckEditorMTGCardMocker.CreateMTGCardModel(name: "2", group: oldValue ),
          DeckEditorMTGCardMocker.CreateMTGCardModel(name: "3", group: oldValue ),
          DeckEditorMTGCardMocker.CreateMTGCardModel(name: "4", group: newValue ),
          DeckEditorMTGCardMocker.CreateMTGCardModel(name: "5", group: newValue ),
        ],
      };

      var oldGroup = viewmodel.Groups.First(x => x.Key == oldValue);
      var oldCardCount = oldGroup.Count;
      var oldGroupCount = viewmodel.Groups.Count;

      var newGroup = viewmodel.Groups.First(x => x.Key == newValue);
      var newCardCount = viewmodel.Cards.Count;
      var newGroupCount = oldCardCount - 1;

      await viewmodel.RenameGroupCommand.ExecuteAsync(oldGroup);

      Assert.AreEqual(newGroupCount, viewmodel.Groups.Count);
      Assert.AreEqual(newCardCount, newGroup.Count);
      Assert.AreEqual(0, oldGroup.Count);

      viewmodel.UndoStack.Undo();

      Assert.AreEqual(oldGroupCount, viewmodel.Groups.Count);
      Assert.AreEqual(oldCardCount, oldGroup.Count);
      Assert.AreEqual(newCardCount - oldCardCount, newGroup.Count);

      viewmodel.UndoStack.Redo();

      Assert.AreEqual(newGroupCount, viewmodel.Groups.Count);
      Assert.AreEqual(newCardCount, newGroup.Count);
      Assert.AreEqual(0, oldGroup.Count);
    }
  }
}
