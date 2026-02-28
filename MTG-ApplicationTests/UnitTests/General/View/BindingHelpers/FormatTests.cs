using MTGApplication.General.Views.BindingHelpers;

namespace MTGApplicationTests.UnitTests.General.View.BindingHelpers;

[TestClass]
public class FormatTests
{
  [TestMethod]
  public void EuroToStringTest() => Assert.AreEqual("19 000,50 €", Format.EuroToString(19000.5f));
}