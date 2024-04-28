using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.ViewModels.Charts;
using MTGApplicationTests.Services;

namespace MTGApplicationTests.ViewModels
{
  [TestClass]
  public class MTGCardModelChartTests
  {
    [TestMethod]
    public void AddItemToSeriesTest()
    {
      var firstCount = 13;
      var secondCount = 5;
      var firstCard = Mocker.MTGCardModelMocker.CreateMTGCardModel(count: firstCount);
      var secondCard = Mocker.MTGCardModelMocker.CreateMTGCardModel(count: secondCount);
      var series = new MTGCardModelCMCSeriesItem(firstCard);

      series.AddItem(secondCard);
      Assert.AreEqual(series.PrimaryValue, firstCount + secondCount);
      Assert.AreEqual(series.SecondaryValue, firstCard.Info.CMC);
    }

    [TestMethod]
    public void RemoveItemFromSeriesTest()
    {
      var firstCount = 13;
      var secondCount = 5;
      var firstCard = Mocker.MTGCardModelMocker.CreateMTGCardModel(count: firstCount);
      var secondCard = Mocker.MTGCardModelMocker.CreateMTGCardModel(count: secondCount);
      var series = new MTGCardModelCMCSeriesItem(firstCard);

      series.AddItem(secondCard);

      series.RemoveItem(firstCard);
      Assert.AreEqual(series.PrimaryValue, secondCount);
      Assert.AreEqual(series.SecondaryValue, firstCard.Info.CMC);
    }

    [TestMethod]
    public void UpdateCardCountInSeriesTest()
    {
      var firstCount = 13;
      var firstCard = Mocker.MTGCardModelMocker.CreateMTGCardModel(count: firstCount);
      var series = new MTGCardModelCMCSeriesItem(firstCard);

      firstCard.Count++;
      Assert.AreEqual(series.PrimaryValue, firstCount + 1);
    }
  }
}