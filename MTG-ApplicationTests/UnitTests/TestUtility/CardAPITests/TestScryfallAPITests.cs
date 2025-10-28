using MTGApplicationTests.TestUtility.Importers;

namespace MTGApplicationTests.UnitTests.TestUtility.CardAPITests;

[TestClass]
public class TestScryfallAPITests
{
  [TestMethod(DisplayName = "API should convert json file into cards")]
  public async Task JsonResponse_ConversionToCard_ReturnCard()
  {
    var result = await new TestScryfallAPI().GetCardsFromSampleJSON();

    Assert.IsNotEmpty(result, "Result should have cards");
  }
}