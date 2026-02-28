using MTGApplicationTests.TestUtility.Mocker;
using MTGApplicationTests.TestUtility.ViewModel;

namespace MTGApplicationTests.UnitTests.Features.DeckEditor.ViewModels.DeckCard;

[TestClass]
public class Properties
{
  [TestMethod]
  public void Init_Name()
  {
    var factory = new TestDeckCardViewModelFactory()
    {
      Model = new(MTGCardInfoMocker.MockInfo(name: "Card"))
    };
    var vm = factory.Build();

    Assert.AreEqual("Card", vm.Name);
  }

  [TestMethod]
  public void Init_Info()
  {
    var info = MTGCardInfoMocker.MockInfo(name: "Card");
    var factory = new TestDeckCardViewModelFactory()
    {
      Model = new(info)
    };
    var vm = factory.Build();

    Assert.AreEqual(info, vm.Info);
  }

  [TestMethod]
  public void Change_ModelInfo_InfoChanged()
  {
    var info = MTGCardInfoMocker.MockInfo(name: "Card");
    var factory = new TestDeckCardViewModelFactory()
    {
      Model = new(info)
    };
    var vm = factory.Build();

    var changed = info with { Name = "Changed" };
    vm.AssertPropertyChanged(nameof(vm.Info), () => factory.Model.Info = changed);
  }

  [TestMethod]
  public void Init_Count()
  {
    var info = MTGCardInfoMocker.MockInfo();
    var factory = new TestDeckCardViewModelFactory()
    {
      Model = new(info) { Count = 4 }
    };
    var vm = factory.Build();

    Assert.AreEqual(4, vm.Count);
  }

  [TestMethod]
  public void Change_ModelCount_CountChanged()
  {
    var info = MTGCardInfoMocker.MockInfo();
    var factory = new TestDeckCardViewModelFactory()
    {
      Model = new(info) { Count = 1 }
    };
    var vm = factory.Build();

    vm.AssertPropertyChanged(nameof(vm.Count), () => factory.Model.Count = 5);
  }

  [TestMethod]
  public void Init_Group()
  {
    var info = MTGCardInfoMocker.MockInfo();
    var factory = new TestDeckCardViewModelFactory()
    {
      Model = new(info) { Group = "Group" }
    };
    var vm = factory.Build();

    Assert.AreEqual("Group", vm.Group);
  }

  [TestMethod]
  public void Change_ModelGroup_GroupChanged()
  {
    var info = MTGCardInfoMocker.MockInfo();
    var factory = new TestDeckCardViewModelFactory()
    {
      Model = new(info) { Group = string.Empty }
    };
    var vm = factory.Build();

    vm.AssertPropertyChanged(nameof(vm.Group), () => factory.Model.Group = "Changed");
  }

  [TestMethod]
  public void Init_CardTag()
  {
    var info = MTGCardInfoMocker.MockInfo();
    var factory = new TestDeckCardViewModelFactory()
    {
      Model = new(info) { CardTag = MTGApplication.General.Models.CardTag.Add }
    };
    var vm = factory.Build();

    Assert.AreEqual(MTGApplication.General.Models.CardTag.Add, vm.CardTag);
  }

  [TestMethod]
  public void Change_ModelCardTag_TagChanged()
  {
    var info = MTGCardInfoMocker.MockInfo();
    var factory = new TestDeckCardViewModelFactory()
    {
      Model = new(info) { CardTag = null }
    };
    var vm = factory.Build();

    vm.AssertPropertyChanged(nameof(vm.CardTag), () => factory.Model.CardTag = MTGApplication.General.Models.CardTag.Add);
  }
}