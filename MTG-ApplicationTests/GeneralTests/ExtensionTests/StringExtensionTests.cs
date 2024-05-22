using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.General.Extensions;

namespace MTGApplicationTests.GeneralTests.ExtensionTests;

[TestClass]
public class StringExtensionTests
{
  [TestMethod]
  public void ToKebabCaseTest() => Assert.AreEqual("This-is-kebab-cased", "This-i.s/ kebab cased".ToKebabCase());
}