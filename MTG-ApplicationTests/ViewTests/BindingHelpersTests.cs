using Microsoft.UI.Xaml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.General.Views.BindingHelpers;

namespace MTGApplicationTests.BindingHelpers;

public class BindingHelpersTests
{
  [TestClass]
  public class ComparisonTests
  {
    [TestMethod]
    public void MoreThanTest() => Assert.IsTrue(Comparison.MoreThan(3, 1));
  
    [TestMethod]
    public void NotNullTest() => Assert.IsFalse(Comparison.NotNull(null));
  
    [TestMethod]
    public void NotNullOrEmptyTest() => Assert.IsFalse(Comparison.NotNullOrEmpty(string.Empty));

    [TestMethod]
    public void EqualsTest() => Assert.IsTrue(Comparison.Equals(1, 1));
  }

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
    public void StringWithDefaultTest_Text() => Assert.AreEqual("asd", Format.StringWithDefault("asd", "default"));

    [TestMethod]
    public void StringWithDefaultTest_Default() => Assert.AreEqual("default", Format.StringWithDefault(string.Empty, "default"));
  }

  [TestClass]
  public class VisibilityHelpersTests
  {
    [TestMethod]
    public void VisibilityInversionTest() => Assert.AreEqual(Visibility.Visible, VisibilityHelpers.VisibilityInversion(Visibility.Collapsed));

    [TestMethod]
    public void BooleanToVisibilityTest() => Assert.AreEqual(Visibility.Visible, VisibilityHelpers.BooleanToVisibility(true));

    [TestMethod]
    public void IntToVisibilityTest() => Assert.AreEqual(Visibility.Collapsed, VisibilityHelpers.IntToVisibility(2, 1));
  }

  [TestClass]
  public class MTGCardViewModelHelpersTests
  {
    [TestMethod]
    public void OwnedToOpacityTest() => Assert.AreEqual(.5f, MTGCardViewModelHelpers.OwnedToOpacity(false));
  }
}