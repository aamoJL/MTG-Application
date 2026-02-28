using MTGApplication.Features.CardCollectionEditor.Models;
using MTGApplicationTests.TestUtility.Importers;
using MTGApplicationTests.TestUtility.Mocker;
using System.Collections.Specialized;

namespace MTGApplicationTests.UnitTests.Features.CardCollectionEditor.ViewModels.CollectionList;

[TestClass]
public class Properties
{
  [TestMethod]
  public void Init_Name()
  {
    var model = new MTGCardCollectionList()
    {
      Name = "Name"
    };
    var factory = new TestCollectionListViewModelFactory();
    var vm = factory.Build(model);

    Assert.AreEqual("Name", vm.Name);
  }

  [TestMethod]
  public void Change_Model_Name()
  {
    var model = new MTGCardCollectionList()
    {
      Name = "Name"
    };
    var factory = new TestCollectionListViewModelFactory();
    var vm = factory.Build(model);

    var changed = false;
    vm.PropertyChanged += (_, _) => { changed = true; };

    model.Name = "New";

    Assert.IsTrue(changed);
  }

  [TestMethod]
  public void Init_Query()
  {
    var model = new MTGCardCollectionList()
    {
      SearchQuery = "Query"
    };
    var factory = new TestCollectionListViewModelFactory();
    var vm = factory.Build(model);

    Assert.AreEqual("Query", vm.Query);
  }

  [TestMethod]
  public void Change_Model_SearchQuery()
  {
    var model = new MTGCardCollectionList()
    {
      SearchQuery = "Query"
    };
    var factory = new TestCollectionListViewModelFactory();
    var vm = factory.Build(model);

    var changed = false;
    vm.PropertyChanged += (_, _) => { changed = true; };

    model.SearchQuery = "New";

    Assert.IsTrue(changed);
  }

  [TestMethod]
  public void Init_Cards()
  {
    var model = new MTGCardCollectionList()
    {
      Cards = [.. MTGCardMocker.Mock(5)]
    };
    var factory = new TestCollectionListViewModelFactory();
    var vm = factory.Build(model);

    Assert.HasCount(5, vm.Cards);
  }

  [TestMethod]
  public void Change_Model_Cards()
  {
    var model = new MTGCardCollectionList()
    {
      Cards = [.. MTGCardMocker.Mock(5)]
    };
    var factory = new TestCollectionListViewModelFactory();
    var vm = factory.Build(model);

    var changed = false;
    ((INotifyCollectionChanged)vm.Cards).CollectionChanged += (_, _) => { changed = true; };

    model.Cards.Add(MTGCardMocker.Mock());

    Assert.IsTrue(changed);
  }

  [TestMethod]
  public void Init_QueryCards()
  {
    var model = new MTGCardCollectionList();
    var factory = new TestCollectionListViewModelFactory();
    var vm = factory.Build(model);

    Assert.HasCount(0, vm.QueryCards.Collection);
    Assert.AreEqual(0, vm.QueryCards.TotalCardCount);
  }

  [TestMethod]
  public async Task QueryCards_IsOwnedChanged_CardsChange()
  {
    var model = new MTGCardCollectionList();
    var factory = new TestCollectionListViewModelFactory()
    {
      Importer = new()
      {
        Result = TestMTGCardImporter.Success([
          new(MTGCardInfoMocker.MockInfo()),
          new(MTGCardInfoMocker.MockInfo()),
          new(MTGCardInfoMocker.MockInfo()),
          new(MTGCardInfoMocker.MockInfo()),
          new(MTGCardInfoMocker.MockInfo()),
        ])
      }
    };
    var vm = factory.Build(model);

    vm.RefreshCommand.Execute(null);

    await vm.QueryCards.Collection.LoadMoreItemsAsync(10);

    Assert.HasCount(5, vm.QueryCards.Collection);

    var card = vm.QueryCards.Collection.First();

    card.SwitchOwnershipCommand.Execute(null);
    Assert.HasCount(1, vm.Cards);

    card.SwitchOwnershipCommand.Execute(null);
    Assert.HasCount(0, vm.Cards);
  }
}