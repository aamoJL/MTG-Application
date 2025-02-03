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
  public class ClearCommandTests
  {
    [TestMethod]
    public void Clear_GroupCardsRemoved()
    {
      var groupKey = "first";
      var cards = new DeckEditorMTGCard[]
      {
        DeckEditorMTGCardMocker.CreateMTGCardModel(name: "1", group: groupKey),
        DeckEditorMTGCardMocker.CreateMTGCardModel(name: "2", group: groupKey),
        DeckEditorMTGCardMocker.CreateMTGCardModel(name: "3", group: groupKey),
        DeckEditorMTGCardMocker.CreateMTGCardModel(name: "4", group: string.Empty),
        DeckEditorMTGCardMocker.CreateMTGCardModel(name: "5", group: string.Empty),
        DeckEditorMTGCardMocker.CreateMTGCardModel(name: "6", group: string.Empty),
      };
      var source = new ObservableCollection<DeckEditorMTGCard>(cards);
      var viewmodel = new CardGroupViewModel(groupKey, source, new TestMTGCardImporter());

      viewmodel.ClearCommand.Execute(null);

      CollectionAssert.AreEquivalent(
        cards.Where(x => x.Group != groupKey).ToArray(), viewmodel.Source.ToArray());
      Assert.AreEqual(0, viewmodel.Count);

      viewmodel.UndoStack.Undo();

      CollectionAssert.AreEquivalent(cards, viewmodel.Source.ToArray());
      Assert.AreEqual(cards.Count(x => x.Group == groupKey), viewmodel.Count);

      viewmodel.UndoStack.Redo();

      CollectionAssert.AreEquivalent(
        cards.Where(x => x.Group != groupKey).ToArray(), viewmodel.Source.ToArray());
      Assert.AreEqual(0, viewmodel.Count);
    }
  }
}
