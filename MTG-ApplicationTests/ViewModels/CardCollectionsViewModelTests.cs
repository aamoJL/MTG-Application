using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Models;
using MTGApplication.ViewModels;
using MTGApplicationTests.API;
using MTGApplicationTests.Services;
using static MTGApplicationTests.Database.InMemoryMTGCardCollectionRepositoryTests;
using static MTGApplicationTests.Services.TestDialogService;

namespace MTGApplicationTests.ViewModels
{
  [TestClass]
  public class CardCollectionsViewModelTests
  {
    #region List Tests
    [TestMethod]
    public async Task NewListTest()
    {
      var list = new MTGCardCollectionList() { Name = "First" };
      var vm = new CardCollectionsViewModel(new TestCardAPI(), new TestInMemoryMTGCardCollectionRepository(new TestCardAPI()))
      {
        Dialogs = new()
        {
          NewCollectionListDialog = new TestCollectionListContentDialog() { ReturnsObject = list }
        }
      };

      await vm.NewCollectionListDialog();
      Assert.AreEqual(1, vm.Collection.CollectionLists.Count);
      Assert.AreEqual(list, vm.Collection.CollectionLists[0]);
      Assert.IsTrue(vm.HasUnsavedChanges);
    }

    [TestMethod]
    public async Task DeleteListTest()
    {
      var list = new MTGCardCollectionList() { Name = "First" };
      var vm = new CardCollectionsViewModel(new TestCardAPI(), new TestInMemoryMTGCardCollectionRepository(new TestCardAPI()))
      {
        Dialogs = new()
        {
          NewCollectionListDialog = new TestCollectionListContentDialog() { ReturnsObject = list},
          DeleteListDialog = new TestConfirmationDialog(Microsoft.UI.Xaml.Controls.ContentDialogResult.Primary)
        }
      };

      await vm.NewCollectionListDialog();

      await vm.DeleteCollectionListDialog();
      Assert.AreEqual(0, vm.Collection.CollectionLists.Count);
      Assert.IsTrue(vm.HasUnsavedChanges);
    }

    [TestMethod]
    public async Task EditListTest()
    {
      var list = new MTGCardCollectionList() { Name = "First" };
      var editedList = new MTGCardCollectionList() { Name = "Edited" };
      var vm = new CardCollectionsViewModel(new TestCardAPI(), new TestInMemoryMTGCardCollectionRepository(new TestCardAPI()))
      {
        Dialogs = new()
        {
          NewCollectionListDialog = new TestCollectionListContentDialog() { ReturnsObject = list },
          EditCollectionListDialog = new TestCollectionListContentDialog() { ReturnsObject = editedList }
        }
      };

      await vm.NewCollectionListDialog();

      await vm.EditCollectionListDialog();
      Assert.AreEqual(1, vm.Collection.CollectionLists.Count);
      Assert.AreEqual(list.Name, editedList.Name);
      Assert.IsTrue(vm.HasUnsavedChanges);
    }

    [TestMethod]
    public void AddToListTest()
    {
      var list = new MTGCardCollectionList() { Name = "First" };
      var vm = new CardCollectionsViewModel(new TestCardAPI(), new TestInMemoryMTGCardCollectionRepository(new TestCardAPI()));

      vm.Collection.CollectionLists.Add(list);
      vm.SelectedList = list;

      var card = Mocker.MTGCardModelMocker.CreateMTGCardModel();
      vm.Collection.CollectionLists[0].AddToList(card);
      Assert.AreEqual(1, vm.Collection.CollectionLists[0].Cards.Count);
      Assert.IsTrue(vm.HasUnsavedChanges);
    }

    [TestMethod]
    public void RemoveFromListTest()
    {
      var list = new MTGCardCollectionList() { Name = "First" };
      var vm = new CardCollectionsViewModel(new TestCardAPI(), new TestInMemoryMTGCardCollectionRepository(new TestCardAPI()));

      vm.Collection.CollectionLists.Add(list);
      var card = Mocker.MTGCardModelMocker.CreateMTGCardModel();
      vm.Collection.CollectionLists[0].Cards.Add(card);
      vm.SelectedList = list;

      vm.Collection.CollectionLists[0].RemoveFromList(card);
      Assert.AreEqual(0, vm.Collection.CollectionLists[0].Cards.Count);
      Assert.IsTrue(vm.HasUnsavedChanges);
    }

    [TestMethod]
    public async Task ChangeListTest()
    {
      var firstlist = new MTGCardCollectionList() { Name = "First" };
      var secondlist = new MTGCardCollectionList() { Name = "Second" };
      var vm = new CardCollectionsViewModel(new TestCardAPI(), new TestInMemoryMTGCardCollectionRepository(new TestCardAPI()))
      {
        Dialogs = new()
        {
          NewCollectionListDialog = new TestCollectionListContentDialog() { ReturnsObject = firstlist },
        }
      };

      await vm.NewCollectionListDialog();
      vm.Collection.CollectionLists.Add(secondlist);
      Assert.AreEqual(firstlist, vm.SelectedList);
      
      vm.ChangeSelectedCollectionList(secondlist);
      Assert.AreEqual(secondlist, vm.SelectedList);
    }
    #endregion

    #region Collection Tests
    [TestMethod]
    public async Task NewCollectionTest()
    {
      var vm = new CardCollectionsViewModel(new TestCardAPI(), new TestInMemoryMTGCardCollectionRepository(new TestCardAPI()));

      await vm.NewCollectionDialog();
      Assert.AreEqual(string.Empty, vm.Collection.Name);
      Assert.AreEqual(0, vm.Collection.CollectionLists.Count);
      Assert.IsFalse(vm.HasUnsavedChanges);
    }

    [TestMethod]
    public async Task OpenCollectionTest()
    {
      var collection = new MTGCardCollection() { Name = "Collection" };
      using var repo = new TestInMemoryMTGCardCollectionRepository(new TestCardAPI());
      var vm = new CardCollectionsViewModel(new TestCardAPI(), repo)
      {
        Dialogs = new()
        {
          LoadDialog = new TestComboBoxDialog(Microsoft.UI.Xaml.Controls.ContentDialogResult.Primary, collection.Name)
        }
      };

      await repo.Add(collection);

      await vm.LoadCollectionDialog();
      Assert.AreEqual(collection.Name, vm.Collection.Name);
      Assert.IsFalse(vm.HasUnsavedChanges);
    }

    [TestMethod]
    public async Task SaveCollectionTest()
    {
      var collectionName = "Collection";
      var list = new MTGCardCollectionList() { Name = "First" };
      using var repo = new TestInMemoryMTGCardCollectionRepository(new TestCardAPI());
      var vm = new CardCollectionsViewModel(new TestCardAPI(), repo)
      {
        Dialogs = new()
        {
          SaveDialog = new TestTextBoxDialog(Microsoft.UI.Xaml.Controls.ContentDialogResult.Primary, collectionName)
        }
      };

      vm.Collection.CollectionLists.Add(list);

      await vm.SaveCollectionDialog();
      Assert.AreEqual(1, (await repo.Get()).ToList().Count);
      Assert.AreEqual(1, (await repo.Get(collectionName)).CollectionLists.Count);
      Assert.IsFalse(vm.HasUnsavedChanges);
    }

    [TestMethod]
    public async Task DeleteCollectionTest()
    {
      var collectionName = "Collection";
      var list = new MTGCardCollectionList() { Name = "First" };
      using var repo = new TestInMemoryMTGCardCollectionRepository(new TestCardAPI());
      var vm = new CardCollectionsViewModel(new TestCardAPI(), repo)
      {
        Dialogs = new()
        {
          SaveDialog = new TestTextBoxDialog(Microsoft.UI.Xaml.Controls.ContentDialogResult.Primary, collectionName),
          DeleteCollectionDialog = new TestConfirmationDialog(Microsoft.UI.Xaml.Controls.ContentDialogResult.Primary)
        }
      };

      vm.Collection.CollectionLists.Add(list);
      await vm.SaveCollectionDialog();

      await vm.DeleteCollectionDialog();
      Assert.AreEqual(0, (await repo.Get()).ToList().Count);
      Assert.IsFalse(vm.HasUnsavedChanges);
    }
    #endregion
  }
}
