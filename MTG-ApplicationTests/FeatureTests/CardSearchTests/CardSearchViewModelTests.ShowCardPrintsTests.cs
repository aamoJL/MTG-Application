using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Features.CardSearch;
using MTGApplication.General.Models.Card;
using MTGApplicationTests.TestUtility.API;
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
      var card = MTGCardModelMocker.CreateMTGCardModel(setCode: "abc");
      var viewmodel = new CardSearchViewModel(new TestCardAPI())
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
