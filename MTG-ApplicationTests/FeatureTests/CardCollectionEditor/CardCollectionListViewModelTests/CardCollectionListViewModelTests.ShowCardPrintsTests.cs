﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Features.CardCollectionEditor.CardCollectionList.Models;
using MTGApplication.General.Models;
using MTGApplicationTests.TestUtility.Mocker;
using MTGApplicationTests.TestUtility.Services;

namespace MTGApplicationTests.FeatureTests.CardCollection.CardCollectionViewModelTests;
public partial class CardCollectionListViewModelTests
{
  [TestClass]
  public class ShowCardPrintsTests : CardCollectionListViewModelTestsBase
  {
    [TestMethod]
    public async Task ShowPrints_PrintConfirmationShown()
    {
      var confirmer = new TestConfirmer<MTGCard, IEnumerable<MTGCard>>();
      var card = new CardCollectionMTGCard(MTGCardInfoMocker.MockInfo());
      var viewmodel = await new Mocker(_dependencies)
      {
        Confirmers = new()
        {
          ShowCardPrintsConfirmer = confirmer
        }
      }.MockVM();

      await viewmodel.ShowCardPrintsCommand.ExecuteAsync(card);

      ConfirmationAssert.ConfirmationShown(confirmer);
    }
  }
}
