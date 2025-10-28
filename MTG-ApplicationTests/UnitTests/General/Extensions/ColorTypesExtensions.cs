using MTGApplication.General.Extensions;

namespace MTGApplicationTests.UnitTests.General.Extensions;

[TestClass]
public class ColorTypesExtensionsTests
{
  [TestMethod]
  public void GetColorTypeName()
  {
    var colorNames = MTGApplication.General.Extensions.ColorTypesExtensions.ColorNames;

    Assert.IsNotEmpty(colorNames);

    foreach (var colorName in colorNames)
      Assert.AreEqual(colorName.Value, colorName.Key.GetFullName());
  }
}
