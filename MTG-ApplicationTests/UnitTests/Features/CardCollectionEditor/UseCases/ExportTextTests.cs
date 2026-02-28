using MTGApplication.Features.CardCollectionEditor.UseCases;
using MTGApplicationTests.TestUtility.Exporters;

namespace MTGApplicationTests.UnitTests.Features.CardCollectionEditor.UseCases;

[TestClass]
public class ExportTextTests
{
  [TestMethod]
  public void Export()
  {
    var expoter = new TestStringExporter()
    {
      Response = true
    };

    new ExportText(expoter).Execute("Text");

    Assert.AreEqual("Text", expoter.Result);
  }
}