using Microsoft.UI.Xaml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.General.Views.BindingHelpers;

namespace MTGApplicationTests.BindingHelpers;

public partial class BindingHelpersTests
{
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
}