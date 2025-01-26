using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplication.Features.DeckEditor.ViewModels;
using MTGApplicationTests.TestUtility.Importers;
using MTGApplicationTests.TestUtility.Mocker;
using System.Collections.ObjectModel;
using static MTGApplication.Features.DeckEditor.CardList.UseCases.CardListViewModelCommands;

namespace MTGApplicationTests.FeatureTests.DeckEditorTests.ViewModel.CardListViewModelTests.GroupedListTests;

public partial class CardGroupViewModelTests
{
  [TestClass]
  public class ChangeCardCountCommandTests
  {
    [TestMethod]
    public void ChangeCount_CountChanged()
    {
      var card = DeckEditorMTGCardMocker.CreateMTGCardModel(group: string.Empty);
      var source = new ObservableCollection<DeckEditorMTGCard>()
      {
        card
      };
      var viewmodel = new CardGroupViewModel(string.Empty, source, new TestMTGCardImporter());

      var oldValue = card.Count;
      var newValue = 2;
      viewmodel.ChangeCardCountCommand.Execute(new CardCountChangeArgs(card, newValue));

      Assert.AreEqual(newValue, card.Count);

      viewmodel.UndoStack.Undo();

      Assert.AreEqual(oldValue, card.Count);

      viewmodel.UndoStack.Redo();

      Assert.AreEqual(newValue, card.Count);
    }
  }
}
