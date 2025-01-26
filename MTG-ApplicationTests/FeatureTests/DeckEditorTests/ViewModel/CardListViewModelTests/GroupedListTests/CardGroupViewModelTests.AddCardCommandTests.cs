using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplication.Features.DeckEditor.Editor.Services;
using MTGApplication.Features.DeckEditor.ViewModels;
using MTGApplicationTests.TestUtility.Importers;
using MTGApplicationTests.TestUtility.Mocker;
using System.Collections.ObjectModel;

namespace MTGApplicationTests.FeatureTests.DeckEditorTests.ViewModel.CardListViewModelTests.GroupedListTests;

public partial class CardGroupViewModelTests
{
  [TestClass]
  public class AddCardCommandTests
  {
    [TestMethod]
    public async Task Add_SameGroup_CardAdded()
    {
      var groupKey = "first";
      var source = new ObservableCollection<DeckEditorMTGCard>();
      var viewmodel = new CardGroupViewModel(groupKey, source, new TestMTGCardImporter());

      var card = DeckEditorMTGCardMocker.CreateMTGCardModel(group: groupKey);
      await viewmodel.AddCardCommand.ExecuteAsync(card);

      Assert.IsTrue(viewmodel.Cards.Contains(card));

      viewmodel.UndoStack.Undo();

      Assert.IsFalse(viewmodel.Cards.Contains(card));

      viewmodel.UndoStack.Redo();

      Assert.IsTrue(viewmodel.Cards.Contains(card));
    }

    [TestMethod]
    public async Task Add_DifferentGroup_CardAddedToGroup()
    {
      var groupKey = "first";
      var source = new ObservableCollection<DeckEditorMTGCard>();
      var viewmodel = new CardGroupViewModel(groupKey, source, new TestMTGCardImporter());

      var card = DeckEditorMTGCardMocker.CreateMTGCardModel(group: "second");
      await viewmodel.AddCardCommand.ExecuteAsync(card);

      Assert.IsTrue(viewmodel.Cards.Contains(card));
      Assert.AreEqual(groupKey, card.Group);

      viewmodel.UndoStack.Undo();

      Assert.IsFalse(viewmodel.Cards.Contains(card));
      Assert.AreNotEqual(groupKey, card.Group);

      viewmodel.UndoStack.Redo();

      Assert.IsTrue(viewmodel.Cards.Contains(card));
      Assert.AreEqual(groupKey, card.Group);
    }

    [TestMethod]
    public async Task Add_Exists_SameGroup_CountCombined()
    {
      var groupKey = "first";
      var card = DeckEditorMTGCardMocker.CreateMTGCardModel(group: groupKey);
      var source = new ObservableCollection<DeckEditorMTGCard>()
      {
        card
      };
      var viewmodel = new CardGroupViewModel(groupKey, source, new TestMTGCardImporter());

      var copy = new DeckEditorMTGCardCopier().Copy(card);
      await viewmodel.AddCardCommand.ExecuteAsync(copy);

      Assert.IsTrue(viewmodel.Cards.Contains(card));
      Assert.IsFalse(viewmodel.Cards.Contains(copy));
      Assert.AreEqual(2, viewmodel.Count);

      viewmodel.UndoStack.Undo();

      Assert.IsTrue(viewmodel.Cards.Contains(card));
      Assert.IsFalse(viewmodel.Cards.Contains(copy));
      Assert.AreEqual(1, viewmodel.Count);

      viewmodel.UndoStack.Redo();

      Assert.IsTrue(viewmodel.Cards.Contains(card));
      Assert.IsFalse(viewmodel.Cards.Contains(copy));
      Assert.AreEqual(2, viewmodel.Count);
    }

    [TestMethod]
    public async Task Add_ExistsInSource_DifferentGroup_CardGroupChanged()
    {
      var groupKey = "first";
      var card = DeckEditorMTGCardMocker.CreateMTGCardModel(group: "second");
      var source = new ObservableCollection<DeckEditorMTGCard>()
      {
        card
      };
      var viewmodel = new CardGroupViewModel(groupKey, source, new TestMTGCardImporter());

      await viewmodel.AddCardCommand.ExecuteAsync(card);

      Assert.IsTrue(viewmodel.Cards.Contains(card));
      Assert.AreEqual(groupKey, card.Group);
      Assert.AreEqual(1, source.Count);

      viewmodel.UndoStack.Undo();

      Assert.IsFalse(viewmodel.Cards.Contains(card));
      Assert.AreNotEqual(groupKey, card.Group);
      Assert.AreEqual(1, source.Count);

      viewmodel.UndoStack.Redo();

      Assert.IsTrue(viewmodel.Cards.Contains(card));
      Assert.AreEqual(groupKey, card.Group);
      Assert.AreEqual(1, source.Count);
    }
  }
}
