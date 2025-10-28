using MTGApplication.Features.CardSearch.ViewModels;
using MTGApplication.General.Models;
using MTGApplicationTests.TestUtility.Importers;
using MTGApplicationTests.TestUtility.Mocker;
using MTGApplicationTests.TestUtility.Services;

namespace MTGApplicationTests.UnitTests.Features.CardSearch.UseCases;

[TestClass]
public class ShowCardPrints
{
  [TestMethod]
  public async Task ChangePrint_ConfirmationShown()
  {
    var confirmer = new TestDataOnlyConfirmer<IEnumerable<MTGCard>>();
    var card = DeckEditorMTGCardMocker.CreateMTGCardModel(setCode: "abc");
    var viewmodel = new CardSearchViewModel(new TestMTGCardImporter())
    {
      Confirmers = new()
      {
        ShowCardPrintsConfirmer = confirmer
      }
    };

    await viewmodel.ShowCardPrintsCommand.ExecuteAsync(card);

    ConfirmationAssert.ConfirmationShown(confirmer);
  }
}
