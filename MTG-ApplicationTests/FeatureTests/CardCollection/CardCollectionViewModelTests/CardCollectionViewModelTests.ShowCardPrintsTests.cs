﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.General.Models.Card;
using MTGApplicationTests.TestUtility.Mocker;
using MTGApplicationTests.TestUtility.Services;

namespace MTGApplicationTests.FeatureTests.CardCollection.CardCollectionViewModelTests;
public partial class CardCollectionViewModelTests
{
  [TestClass]
  public class ShowCardPrintsTests : CardCollectionViewModelTestsBase
  {
    [TestMethod]
    public async Task ShowPrints_PrintConfirmationShown()
    {
      var card = MTGCardModelMocker.CreateMTGCardModel();
      var viewmodel = new Mocker(_dependencies)
      {
        Confirmers = new()
        {
          ShowCardPrintsConfirmer = new TestExceptionConfirmer<MTGCard, IEnumerable<MTGCard>>()
        }
      }.MockVM();

      await ConfirmationAssert.ConfirmationShown(() => viewmodel.ShowCardPrintsCommand.ExecuteAsync(card));
    }
  }
}
