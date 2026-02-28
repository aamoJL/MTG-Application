using MTGApplication.Features.CardCollectionEditor.Models;
using MTGApplicationTests.TestUtility.Importers;
using MTGApplicationTests.TestUtility.Mocker;

namespace MTGApplicationTests.UnitTests.Features.CardCollectionEditor.ViewModels.Collection;

[TestClass]
public class ChangeList
{
  [TestMethod]
  public async Task Change_ToNull_IsNull()
  {
    var model = new MTGCardCollection()
    {
      CollectionLists = [
        new(){Name = "Name 1"},
        new(){Name = "Name 2"},
        new(){Name = "Name 3"},
      ]
    };
    var factory = new TestCollectionViewModelFactory();
    var vm = factory.Build(model);

    await vm.ChangeListCommand.ExecuteAsync(null);

    Assert.IsNull(vm.ListViewModel);
  }

  [TestMethod]
  public async Task Change_ToNotNull_IsSelected()
  {
    var model = new MTGCardCollection()
    {
      CollectionLists = [
        new(){Name = "Name 1"},
        new(){Name = "Name 2"},
        new(){Name = "Name 3"},
      ]
    };
    var factory = new TestCollectionViewModelFactory()
    {
      Notifier = new(),
      Importer = new()
      {
        Result = TestMTGCardImporter.Success()
      },
    };
    var vm = factory.Build(model);

    await vm.ChangeListCommand.ExecuteAsync(vm.CollectionListViewModels[1]);

    Assert.AreEqual(vm.CollectionListViewModels[1], vm.ListViewModel);
  }

  [TestMethod]
  public async Task Change_CardsLoaded()
  {
    var model = new MTGCardCollection()
    {
      CollectionLists = [
        new(){Name = "Name 1"},
        new(){Name = "Name 2"},
        new(){Name = "Name 3"},
      ]
    };
    var factory = new TestCollectionViewModelFactory()
    {
      Importer = new()
      {
        Result = TestMTGCardImporter.Success([
          new(MTGCardInfoMocker.MockInfo()),
          new(MTGCardInfoMocker.MockInfo()),
          new(MTGCardInfoMocker.MockInfo()),
        ]),
      }
    };
    var vm = factory.Build(model);

    await vm.ChangeListCommand.ExecuteAsync(vm.CollectionListViewModels[1]);

    Assert.AreEqual(3, vm.ListViewModel.QueryCards.TotalCardCount);
  }
}