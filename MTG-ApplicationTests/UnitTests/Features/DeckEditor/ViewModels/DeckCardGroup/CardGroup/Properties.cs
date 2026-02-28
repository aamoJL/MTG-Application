using MTGApplicationTests.TestUtility.Mocker;
using MTGApplicationTests.TestUtility.ViewModel;

namespace MTGApplicationTests.UnitTests.Features.DeckEditor.ViewModels.DeckCardGroup.CardGroup;

[TestClass]
public class Properties
{
  [TestMethod]
  public void Init_GroupKey()
  {
    var factory = new TestDeckCardGroupViewModelFactory()
    {
      Model = new("Group", [])
    };
    var vm = factory.Build();

    Assert.AreEqual("Group", vm.GroupKey);
  }

  [TestMethod]
  public void Change_ModelGroupKey_GroupKeyChanged()
  {
    var factory = new TestDeckCardGroupViewModelFactory()
    {
      Model = new("Group", [])
    };
    var vm = factory.Build();

    vm.AssertPropertyChanged(nameof(vm.GroupKey), () => factory.Model.GroupKey = "Changed");
    Assert.AreEqual("Changed", vm.GroupKey);
  }

  [TestMethod]
  public void Init_Size()
  {
    var factory = new TestDeckCardGroupViewModelFactory()
    {
      Model = new("Group", [
        new(MTGCardInfoMocker.MockInfo()){Count = 1, Group = "Group"},
        new(MTGCardInfoMocker.MockInfo()){Count = 2, Group = string.Empty},
        new(MTGCardInfoMocker.MockInfo()){Count = 3, Group = "Group"},
      ])
    };
    var vm = factory.Build();

    Assert.AreEqual(4, vm.Size);
  }

  [TestMethod]
  public void Change_ModelCards_SizeChanges()
  {
    var factory = new TestDeckCardGroupViewModelFactory()
    {
      Model = new("Group", [
        new(MTGCardInfoMocker.MockInfo()){Count = 1, Group = "Group"},
      ])
    };
    var vm = factory.Build();

    vm.AssertPropertyChanged(nameof(vm.Size),
      () => factory.Model.AddToSource(new(MTGCardInfoMocker.MockInfo()) { Count = 2, Group = "Group" }));

    Assert.AreEqual(3, vm.Size);
  }

  [TestMethod]
  public void Change_ModelCardCount_SizeChanges()
  {
    var factory = new TestDeckCardGroupViewModelFactory()
    {
      Model = new("Group", [
        new(MTGCardInfoMocker.MockInfo()){Count = 1, Group = "Group"},
      ])
    };
    var vm = factory.Build();

    vm.AssertPropertyChanged(nameof(vm.Size),
      () => factory.Model.Cards.First().Count = 3);

    Assert.AreEqual(3, vm.Size);
  }

  [TestMethod]
  public void Change_ModelCardGroup_SizeChanges()
  {
    var factory = new TestDeckCardGroupViewModelFactory()
    {
      Model = new("Group", [
        new(MTGCardInfoMocker.MockInfo()){Count = 1, Group = "Group"},
      ])
    };
    var vm = factory.Build();

    vm.AssertPropertyChanged(nameof(vm.Size),
      () => factory.Model.Cards.First().Group = string.Empty);

    Assert.AreEqual(0, vm.Size);
  }

  [TestMethod]
  public void Init_CardViewModels()
  {
    var factory = new TestDeckCardGroupViewModelFactory()
    {
      Model = new("Group", [
        new(MTGCardInfoMocker.MockInfo()){Count = 1, Group = "Group"},
        new(MTGCardInfoMocker.MockInfo()){Count = 2, Group = string.Empty},
        new(MTGCardInfoMocker.MockInfo()){Count = 3, Group = "Group"},
      ])
    };
    var vm = factory.Build();

    Assert.HasCount(2, vm.CardViewModels);
  }

  [TestMethod]
  public void Change_ModelCards_CardViewModelsChanged()
  {
    var factory = new TestDeckCardGroupViewModelFactory()
    {
      Model = new("Group", [
        new(MTGCardInfoMocker.MockInfo()){Count = 1, Group = "Group"},
      ])
    };
    var vm = factory.Build();

    factory.Model.AddToSource(new(MTGCardInfoMocker.MockInfo()) { Count = 2, Group = "Group" });

    Assert.HasCount(2, vm.CardViewModels);
  }
}