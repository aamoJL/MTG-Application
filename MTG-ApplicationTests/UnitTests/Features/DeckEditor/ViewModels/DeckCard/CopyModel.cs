using MTGApplicationTests.TestUtility.Mocker;

namespace MTGApplicationTests.UnitTests.Features.DeckEditor.ViewModels.DeckCard;

[TestClass]
public class CopyModel
{
  [TestMethod]
  public void Copy()
  {
    var factory = new TestDeckCardViewModelFactory()
    {
      Model = new(MTGCardInfoMocker.MockInfo(name: "Card"))
      {
        Count = 3,
        Group = "Group",
        CardTag = MTGApplication.General.Models.CardTag.Add
      }
    };
    var vm = factory.Build();

    var result = vm.CopyModel();

    Assert.AreNotEqual(factory.Model, result);
    Assert.AreEqual(factory.Model.Info, result.Info);
    Assert.AreEqual(factory.Model.Count, result.Count);
    Assert.AreEqual(factory.Model.Group, result.Group);
    Assert.AreEqual(factory.Model.CardTag, result.CardTag);
  }
}
