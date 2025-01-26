using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplication.Features.DeckEditor.ViewModels;
using MTGApplication.General.Models;
using MTGApplicationTests.TestUtility.Importers;
using MTGApplicationTests.TestUtility.Mocker;
using MTGApplicationTests.TestUtility.Services;
using System.Collections.ObjectModel;

namespace MTGApplicationTests.FeatureTests.DeckEditorTests.ViewModel.CardListViewModelTests.GroupedListTests;

public partial class CardGroupViewModelTests
{
  [TestClass]
  public class ChangeCardPrintCommandTests
  {
    [TestMethod]
    public async Task ChangePrint_ConfirmationShown()
    {
      var confirmer = new TestConfirmer<MTGCard, IEnumerable<MTGCard>>();
      var card = DeckEditorMTGCardMocker.CreateMTGCardModel(group: string.Empty);
      var source = new ObservableCollection<DeckEditorMTGCard>()
      {
        card
      };
      var viewmodel = new CardGroupViewModel(string.Empty, source, new TestMTGCardImporter())
      {
        Confirmers = new()
        {
          ChangeCardPrintConfirmer = confirmer
        }
      };

      await viewmodel.ChangeCardPrintCommand.ExecuteAsync(card);

      ConfirmationAssert.ConfirmationShown(confirmer);
    }

    [TestMethod]
    public async Task ChangePrint_PrintChanged()
    {
      var card = DeckEditorMTGCardMocker.CreateMTGCardModel(setCode: "old", price: 1f, group: string.Empty);
      var newCard = DeckEditorMTGCardMocker.CreateMTGCardModel(setCode: "new", price: 999f);
      var source = new ObservableCollection<DeckEditorMTGCard>()
      {
        card
      };
      var viewmodel = new CardGroupViewModel(string.Empty, source, new TestMTGCardImporter() { })
      {
        Confirmers = new()
        {
          ChangeCardPrintConfirmer = new()
          {
            OnConfirm = async (_) => await Task.FromResult(newCard)
          }
        }
      };

      var oldValue = card.Info;
      var newValue = newCard.Info;

      await viewmodel.ChangeCardPrintCommand.ExecuteAsync(card);

      Assert.AreEqual(newValue, card.Info);

      viewmodel.UndoStack.Undo();

      Assert.AreEqual(oldValue, card.Info);

      viewmodel.UndoStack.Redo();

      Assert.AreEqual(newValue, card.Info);
    }
  }
}
