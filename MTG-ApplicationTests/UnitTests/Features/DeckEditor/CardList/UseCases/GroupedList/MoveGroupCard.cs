using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplication.Features.DeckEditor.ViewModels;
using MTGApplicationTests.TestUtility.Importers;
using MTGApplicationTests.TestUtility.Mocker;
using System.Collections.ObjectModel;

namespace MTGApplicationTests.UnitTests.Features.DeckEditor.CardList.UseCases.GroupedList;

public partial class MoveGroupCard
{
  [TestClass]
  public class MoveFrom
  {
    [TestMethod]
    public void MoveFrom_ToDifferentSource_CardRemoved()
    {
      var groupKey = "first";
      var card = DeckEditorMTGCardMocker.CreateMTGCardModel(group: groupKey);
      var source = new ObservableCollection<DeckEditorMTGCard>()
      {
        card
      };
      var viewmodel = new CardGroupViewModel(groupKey, source, new TestMTGCardImporter());

      viewmodel.BeginMoveFromCommand.Execute(card);
      viewmodel.ExecuteMoveCommand.Execute(card);

      Assert.DoesNotContain(card, viewmodel.Cards);

      viewmodel.UndoStack.Undo();

      Assert.Contains(card, viewmodel.Cards);

      viewmodel.UndoStack.Redo();

      Assert.DoesNotContain(card, viewmodel.Cards);
    }

    [TestMethod]
    public async Task MoveFrom_ToSameSource_CardRemoved()
    {
      var groupKey = "first";
      var card = DeckEditorMTGCardMocker.CreateMTGCardModel(group: groupKey);
      var source = new ObservableCollection<DeckEditorMTGCard>()
      {
        card
      };
      var viewmodel = new CardGroupViewModel(groupKey, source, new TestMTGCardImporter());
      var target = new CardGroupViewModel("second", source, new TestMTGCardImporter());

      viewmodel.BeginMoveFromCommand.Execute(card);
      await target.BeginMoveToCommand.ExecuteAsync(card);
      viewmodel.ExecuteMoveCommand.Execute(card);

      Assert.DoesNotContain(card, viewmodel.Cards);

      viewmodel.UndoStack.Undo();

      Assert.Contains(card, viewmodel.Cards);

      viewmodel.UndoStack.Redo();

      Assert.DoesNotContain(card, viewmodel.Cards);
    }
  }

  [TestClass]
  public class MoveTo
  {
    [TestMethod]
    public async Task MoveTo_FromDifferentSource_CardAdded()
    {
      var groupKey = "first";
      var card = DeckEditorMTGCardMocker.CreateMTGCardModel(group: string.Empty);
      var source = new ObservableCollection<DeckEditorMTGCard>();
      var viewmodel = new CardGroupViewModel(groupKey, source, new TestMTGCardImporter());

      await viewmodel.BeginMoveToCommand.ExecuteAsync(card);
      viewmodel.ExecuteMoveCommand.Execute(card);

      Assert.Contains(card, viewmodel.Cards);

      viewmodel.UndoStack.Undo();

      Assert.DoesNotContain(card, viewmodel.Cards);

      viewmodel.UndoStack.Redo();

      Assert.Contains(card, viewmodel.Cards);
    }

    [TestMethod]
    public async Task MoveTo_FromSameSource_GroupChanged()
    {
      var groupKey = "first";
      var card = DeckEditorMTGCardMocker.CreateMTGCardModel(group: string.Empty);
      var source = new ObservableCollection<DeckEditorMTGCard>()
      {
        card
      };
      var viewmodel = new CardGroupViewModel(groupKey, source, new TestMTGCardImporter());
      var origin = new CardGroupViewModel(string.Empty, source, new TestMTGCardImporter());

      origin.BeginMoveFromCommand.Execute(card);
      await viewmodel.BeginMoveToCommand.ExecuteAsync(card);
      viewmodel.ExecuteMoveCommand.Execute(card);

      Assert.Contains(card, viewmodel.Cards);

      viewmodel.UndoStack.Undo();

      Assert.DoesNotContain(card, viewmodel.Cards);

      viewmodel.UndoStack.Redo();

      Assert.Contains(card, viewmodel.Cards);
    }
  }
}
