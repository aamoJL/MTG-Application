using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.General.Views.BindingHelpers;

namespace MTGApplicationTests.BindingHelpers;

public partial class BindingHelpersTests
{
  [TestClass]
  public class FormatTests
  {
    [TestMethod]
    public void EuroToStringTest() => Assert.AreEqual("19 000,50 €", Format.EuroToString(19000.5f));

    [TestMethod]
    public void EuroToStringTest_Digits() => Assert.AreEqual("19 000,00 €", Format.EuroToString(19000.5f, 0));

    [TestMethod]
    public void ToUpperTest() => Assert.AreEqual("ASD", Format.ToUpper("Asd"));

    [TestMethod]
    public void StringWithDefaultTest_Text() => Assert.AreEqual("asd", Format.ValueOrDefault("asd", "default"));

    [TestMethod]
    public void StringWithDefaultTest_Default() => Assert.AreEqual("default", Format.ValueOrDefault(string.Empty, "default"));
  }
}