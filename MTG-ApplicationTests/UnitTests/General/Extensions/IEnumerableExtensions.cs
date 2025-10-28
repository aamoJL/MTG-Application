using MTGApplication.General.Extensions;

namespace MTGApplicationTests.UnitTests.General.Extensions;

[TestClass]
public class IEnumerableExtensionsTests
{
  [TestMethod]
  public void FindPosition_EmptyArray()
  {
    var array = Array.Empty<int>();
    var result = array.FindPosition(4);

    Assert.AreEqual(0, result);
  }

  [TestMethod]
  public void FindPosition()
  {
    var array = new[] { 0, 1, 2, 3, 4, 5 };

    for (var i = 0; i < array.Length + 1; i++)
    {
      var result = array.FindPosition(i);

      Assert.AreEqual(i, result);
    }
  }
}