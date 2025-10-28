using MTGApplication.General.Extensions;

namespace MTGApplicationTests.UnitTests.General.Extensions;

[TestClass]
public class StringExtensionsTests
{
  [TestMethod]
  public void ToKebabCaseTest() => Assert.AreEqual("This-is-kebab-cased", "This-i.s/ kebab cased".ToKebabCase());
}