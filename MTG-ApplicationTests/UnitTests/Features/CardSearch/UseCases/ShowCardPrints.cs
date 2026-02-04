using MTGApplication.Features.CardSearch.ViewModels;
using MTGApplicationTests.TestUtility.Importers;
using MTGApplicationTests.TestUtility.Mocker;

namespace MTGApplicationTests.UnitTests.Features.CardSearch.UseCases;

[TestClass]
public class ShowCardPrints
{
  [TestMethod]
  public async Task ChangePrint_ConfirmationShown()
  {
    var confirmed = false;
    var card = DeckEditorMTGCardMocker.CreateMTGCardModel(setCode: "abc");
    var viewmodel = new CardSearchPageViewModel(new TestMTGCardImporter())
    {
      ConfirmCardPrints_UC = async (_) => { confirmed = true; },
    };

    await viewmodel.ShowCardPrintsCommand.ExecuteAsync(card);

    Assert.IsTrue(confirmed);
  }
}
