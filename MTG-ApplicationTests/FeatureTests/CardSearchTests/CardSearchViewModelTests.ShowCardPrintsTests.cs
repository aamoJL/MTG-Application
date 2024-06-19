using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Features.CardSearch.ViewModels;
using MTGApplication.General.Models;
using MTGApplicationTests.TestUtility.Importers;
using MTGApplicationTests.TestUtility.Mocker;
using MTGApplicationTests.TestUtility.Services;

namespace MTGApplicationTests.FeatureTests.CardSearchTests;

public partial class CardSearchViewModelTests
{
  [TestClass]
  public class ShowCardPrintsTests
  {
    [TestMethod]
    public async Task ChangePrint_ConfirmationShown()
    {
      var card = DeckEditorMTGCardMocker.CreateMTGCardModel(setCode: "abc");
      var viewmodel = new CardSearchViewModel(new TestMTGCardImporter())
      {
        Confirmers = new()
        {
          ShowCardPrintsConfirmer = new TestExceptionConfirmer<MTGCard, IEnumerable<MTGCard>>()
        }
      };

      await ConfirmationAssert.ConfirmationShown(() => viewmodel.ShowCardPrintsCommand.ExecuteAsync(card));
    }
  }
}
