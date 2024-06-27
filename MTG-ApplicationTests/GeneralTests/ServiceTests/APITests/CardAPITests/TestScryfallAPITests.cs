using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplicationTests.TestUtility.Importers;

namespace MTGApplicationTests.GeneralTests.ServiceTests.APITests.CardAPITests;
[TestClass]
public class TestScryfallAPITests
{
  [TestMethod("API should convert json file into cards")]
  public async Task JsonResponse_ConversionToCard_ReturnCard()
  {
    var result = await new TestScryfallAPI().GetCardsFromSampleJSON();

    Assert.IsTrue(result.Length > 0, "Result should have cards");
  }
}