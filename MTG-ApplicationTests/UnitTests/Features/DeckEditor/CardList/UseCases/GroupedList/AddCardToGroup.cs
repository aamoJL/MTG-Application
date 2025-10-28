using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplication.Features.DeckEditor.ViewModels;
using MTGApplicationTests.TestUtility.Importers;
using MTGApplicationTests.TestUtility.Mocker;
using System.Collections.ObjectModel;

namespace MTGApplicationTests.UnitTests.Features.DeckEditor.CardList.UseCases.GroupedList;

[TestClass]
public class AddCardToGroup
{
  [TestMethod]
  public async Task Add_SameGroup_CardAdded()
  {
    var groupKey = "first";
    var source = new ObservableCollection<DeckEditorMTGCard>();
    var viewmodel = new CardGroupViewModel(groupKey, source, new TestMTGCardImporter());

    var card = DeckEditorMTGCardMocker.CreateMTGCardModel(group: groupKey);
    await viewmodel.AddCardCommand.ExecuteAsync(card);

    Assert.Contains(card, viewmodel.Cards);

    viewmodel.UndoStack.Undo();

    Assert.DoesNotContain(card, viewmodel.Cards);

    viewmodel.UndoStack.Redo();

    Assert.Contains(card, viewmodel.Cards);
  }

  [TestMethod]
  public async Task Add_DifferentGroup_CardAddedToGroup()
  {
    var groupKey = "first";
    var source = new ObservableCollection<DeckEditorMTGCard>();
    var viewmodel = new CardGroupViewModel(groupKey, source, new TestMTGCardImporter());

    var card = DeckEditorMTGCardMocker.CreateMTGCardModel(group: "second");
    await viewmodel.AddCardCommand.ExecuteAsync(card);

    Assert.Contains(card, viewmodel.Cards);
    Assert.AreEqual(groupKey, card.Group);

    viewmodel.UndoStack.Undo();

    Assert.DoesNotContain(card, viewmodel.Cards);
    Assert.AreNotEqual(groupKey, card.Group);

    viewmodel.UndoStack.Redo();

    Assert.Contains(card, viewmodel.Cards);
    Assert.AreEqual(groupKey, card.Group);
  }

  [TestMethod]
  public async Task Add_Exists_SameGroup_CountCombined()
  {
    var groupKey = "first";
    var card = DeckEditorMTGCardMocker.CreateMTGCardModel(group: groupKey);
    var source = new ObservableCollection<DeckEditorMTGCard>() { card };
    var viewmodel = new CardGroupViewModel(groupKey, source, new TestMTGCardImporter());

    var copy = DeckEditorMTGCardMocker.CreateMTGCardModel(group: groupKey);
    await viewmodel.AddCardCommand.ExecuteAsync(copy);

    Assert.Contains(card, viewmodel.Cards);
    Assert.DoesNotContain(copy, viewmodel.Cards);
    Assert.AreEqual(2, viewmodel.Count);

    viewmodel.UndoStack.Undo();

    Assert.Contains(card, viewmodel.Cards);
    Assert.DoesNotContain(copy, viewmodel.Cards);
    Assert.AreEqual(1, viewmodel.Count);

    viewmodel.UndoStack.Redo();

    Assert.Contains(card, viewmodel.Cards);
    Assert.DoesNotContain(copy, viewmodel.Cards);
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

    Assert.Contains(card, viewmodel.Cards);
    Assert.AreEqual(groupKey, card.Group);
    Assert.HasCount(1, source);

    viewmodel.UndoStack.Undo();

    Assert.DoesNotContain(card, viewmodel.Cards);
    Assert.AreNotEqual(groupKey, card.Group);
    Assert.HasCount(1, source);

    viewmodel.UndoStack.Redo();

    Assert.Contains(card, viewmodel.Cards);
    Assert.AreEqual(groupKey, card.Group);
    Assert.HasCount(1, source);
  }
}
