using MTGApplication.Features.DeckEditor.UseCases;
using MTGApplicationTests.TestUtility.Exporters;

namespace MTGApplicationTests.UnitTests.Features.DeckEditor.UseCases;

[TestClass]
public class ExportTextTests
{
  [TestMethod]
  public async Task Export_Exported()
  {
    var exporter = new TestStringExporter()
    {
      Response = true,
    };
    await new ExportText(exporter).Execute("Export text");

    Assert.AreEqual("Export text", exporter.Result);
  }
}
