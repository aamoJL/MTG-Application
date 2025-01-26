using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplication.Features.DeckEditor.ViewModels;
using MTGApplicationTests.TestUtility.Importers;
using MTGApplicationTests.TestUtility.Mocker;
using System.Collections.ObjectModel;

namespace MTGApplicationTests.FeatureTests.DeckEditorTests.ViewModel.CardListViewModelTests.GroupedListTests;

public partial class CardGroupViewModelTests
{
  public partial class MoveTests
  {
    [TestClass]
    public class MoveFromCommandTests
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

        Assert.IsFalse(viewmodel.Cards.Contains(card));

        viewmodel.UndoStack.Undo();

        Assert.IsTrue(viewmodel.Cards.Contains(card));

        viewmodel.UndoStack.Redo();

        Assert.IsFalse(viewmodel.Cards.Contains(card));
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

        Assert.IsFalse(viewmodel.Cards.Contains(card));

        viewmodel.UndoStack.Undo();

        Assert.IsTrue(viewmodel.Cards.Contains(card));

        viewmodel.UndoStack.Redo();

        Assert.IsFalse(viewmodel.Cards.Contains(card));
      }
    }

    [TestClass]
    public class MoveToCommandTests
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

        Assert.IsTrue(viewmodel.Cards.Contains(card));

        viewmodel.UndoStack.Undo();

        Assert.IsFalse(viewmodel.Cards.Contains(card));

        viewmodel.UndoStack.Redo();

        Assert.IsTrue(viewmodel.Cards.Contains(card));
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

        Assert.IsTrue(viewmodel.Cards.Contains(card));

        viewmodel.UndoStack.Undo();

        Assert.IsFalse(viewmodel.Cards.Contains(card));

        viewmodel.UndoStack.Redo();

        Assert.IsTrue(viewmodel.Cards.Contains(card));
      }
    }
  }
}
