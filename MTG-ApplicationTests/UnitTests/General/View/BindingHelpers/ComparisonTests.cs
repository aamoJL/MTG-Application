using MTGApplication.General.Views.BindingHelpers;

namespace MTGApplicationTests.UnitTests.General.View.BindingHelpers;

[TestClass]
public class ComparisonTests
{
  [TestMethod]
  public void MoreThanTest() => Assert.IsTrue(Comparison.MoreThan(3, 1));
}