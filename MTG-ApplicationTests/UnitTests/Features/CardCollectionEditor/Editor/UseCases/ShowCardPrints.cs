using MTGApplication.Features.CardCollectionEditor.CardCollectionList.Models;
using MTGApplication.General.Models;
using MTGApplicationTests.TestUtility.Mocker;
using MTGApplicationTests.TestUtility.Services;
using MTGApplicationTests.UnitTests.Features.CardCollectionEditor.Editor.ViewModels;

namespace MTGApplicationTests.UnitTests.Features.CardCollectionEditor.Editor.UseCases;

[TestClass]
public class ShowCardPrints : CardCollectionEditorViewModelTestBase
{
  [TestMethod]
  public async Task ShowPrints_PrintConfirmationShown()
  {
    var confirmer = new TestConfirmer<MTGCard, IEnumerable<MTGCard>>();
    var card = new CardCollectionMTGCard(MTGCardInfoMocker.MockInfo());
    var viewmodel = new Mocker(_dependencies)
    {
      Confirmers = new()
      {
        CardCollectionConfirmers = new()
        {
          ShowCardPrintsConfirmer = confirmer
        }
      }
    }.MockVM();

    await viewmodel.ShowCardPrintsCommand.ExecuteAsync(card);

    ConfirmationAssert.ConfirmationShown(confirmer);
  }
}
