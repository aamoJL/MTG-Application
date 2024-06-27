using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.General.Extensions;

namespace MTGApplicationTests.GeneralTests.ExtensionTests;

[TestClass]
public class ColorTypesExtensionsTests
{
  [TestMethod]
  public void GetColorTypeName()
  {
    var colorNames = ColorTypesExtensions.ColorNames;

    Assert.IsTrue(colorNames.Count > 0);

    foreach (var colorName in colorNames)
      Assert.AreEqual(colorName.Value, colorName.Key.GetFullName());
  }
}
