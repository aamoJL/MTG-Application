using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.General.Extensions;

namespace MTGApplicationTests.Extensions;

[TestClass]
public class StringExtensionTests
{
  [TestMethod]
  public void ToKebabCaseTest() => Assert.AreEqual("This-is-kebab-cased", StringExtensions.ToKebabCase("This-i.s/ kebab cased"));
}
