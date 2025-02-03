using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplication.Features.DeckEditor.ViewModels;
using MTGApplicationTests.TestUtility.Importers;
using MTGApplicationTests.TestUtility.Mocker;
using System.Collections.ObjectModel;

namespace MTGApplicationTests.FeatureTests.DeckEditorTests.ViewModel.CardListViewModelTests.GroupedListTests;

public partial class CardGroupViewModelTests
{
  [TestClass]
  public class RemoveCardCommand
  {
    [TestMethod]
    public void Remove_CardRemoved()
    {
      var groupKey = "first";
      var card = DeckEditorMTGCardMocker.CreateMTGCardModel(group: groupKey);
      var source = new ObservableCollection<DeckEditorMTGCard>()
      {
        card
      };
      var viewmodel = new CardGroupViewModel(groupKey, source, new TestMTGCardImporter());

      viewmodel.RemoveCardCommand.Execute(card);

      Assert.IsFalse(viewmodel.Cards.Contains(card));
      Assert.AreEqual(0, source.Count);

      viewmodel.UndoStack.Undo();

      Assert.IsTrue(viewmodel.Cards.Contains(card));
      Assert.AreEqual(1, source.Count);

      viewmodel.UndoStack.Redo();

      Assert.IsFalse(viewmodel.Cards.Contains(card));
      Assert.AreEqual(0, source.Count);
    }
  }
}
