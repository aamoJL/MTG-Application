using Microsoft.UI.Xaml;
using MTGApplication.General.Views.BindingHelpers;

namespace MTGApplicationTests.UnitTests.General.View.BindingHelpers;

[TestClass]
public class VisibilityHelpersTests
{
  [TestMethod]
  public void VisibilityInversionTest() => Assert.AreEqual(Visibility.Visible, VisibilityHelpers.VisibilityInversion(Visibility.Collapsed));

  [TestMethod]
  public void BooleanToVisibilityTest() => Assert.AreEqual(Visibility.Visible, VisibilityHelpers.BooleanToVisibility(true));
}