using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Features.CardCollectionEditor.CardCollectionList.Models;
using MTGApplication.General.Models;
using MTGApplicationTests.TestUtility.Mocker;
using MTGApplicationTests.TestUtility.Services;
using static MTGApplicationTests.FeatureTests.CardCollectionEditor.CardCollectionEditorViewModelTests.CardCollectionEditorViewModelTests;

namespace MTGApplicationTests.FeatureTests.CardCollection.CardCollectionViewModelTests;
public partial class CardCollectionEditorViewModelTests
{
  [TestClass]
  public class ShowCardPrintsTests : CardCollectionEditorViewModelTestsBase
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
}
