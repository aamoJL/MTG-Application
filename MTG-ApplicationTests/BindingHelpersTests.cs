using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Views.BindingHelpers;

namespace MTGApplicationTests.BindingHelpers
{
  [TestClass]
  public class BindingHelpersTests
  {
    [TestMethod]
    public void MoreThanTest() => Assert.IsTrue(Comparison.MoreThan(3, 1));
    
    [TestMethod]
    public void NotNullTest() => Assert.IsFalse(Comparison.NotNull(null));
    
    [TestMethod]
    public void NotNullOrEmptyTest() => Assert.IsFalse(Comparison.NotNullOrEmpty(string.Empty));
    
    [TestMethod]
    public void EuroToStringTest() => Assert.AreEqual("19 000,50 €", Format.EuroToString(19000.5f));
  }
}