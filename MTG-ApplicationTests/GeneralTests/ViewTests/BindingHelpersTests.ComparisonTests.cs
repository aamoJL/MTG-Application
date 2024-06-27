using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.General.Views.BindingHelpers;

namespace MTGApplicationTests.BindingHelpers;

public partial class BindingHelpersTests
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
}