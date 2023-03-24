using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Models;
using MTGApplication.ViewModels;
using MTGApplicationTests.API;
using MTGApplicationTests.Services;
using static MTGApplication.ViewModels.CardCollectionsViewModel;
using static MTGApplication.Views.Dialogs;
using static MTGApplicationTests.Database.InMemoryMTGCardCollectionRepositoryTests;
using static MTGApplicationTests.Services.TestDialogService;

namespace MTGApplicationTests.ViewModels
{
  [TestClass]
  public class CardCollectionsViewModelTests
  {
    public class TestCardCollectionsDialogs : CardCollectionsDialogs
    {
      public TestDialogResult<(string Name, string Query)> EditCollectionListDialog { protected get; set; } = new();
      public TestDialogResult<(string Name, string Query)> NewCollectionListDialog { protected get; set; } = new();
      public TestDialogResult<MTGCardViewModel> CardPrintDialog { protected get; set; } = new();
      public TestDialogResult<string> LoadDialog { protected get; set; } = new();
      public TestDialogResult<string> SaveDialog { protected get; set; } = new();
      public TestDialogResult DeleteCollectionDialog { protected get; set; } = new();
      public TestDialogResult SaveUnsavedDialog { protected get; set; } = new();
      public TestDialogResult DeleteListDialog { protected get; set; } = new();
      public TestDialogResult OverrideDialog { protected get; set; } = new();

      public override CollectionListContentDialog GetEditCollectionListDialog(string nameInputText, string queryInputText)
      {
        DialogWrapper = new TestDialogWrapper(EditCollectionListDialog.Result);
        var dialog = base.GetEditCollectionListDialog(EditCollectionListDialog.Values.Name, EditCollectionListDialog.Values.Query);
        return dialog;
      }
      public override CollectionListContentDialog GetNewCollectionListDialog()
      {
        DialogWrapper = new TestDialogWrapper(NewCollectionListDialog.Result);
        var dialog = base.GetNewCollectionListDialog();
        dialog.NameInputText = NewCollectionListDialog.Values.Name;
        dialog.QueryInputText = NewCollectionListDialog.Values.Query;
        return dialog;
      }
      public override ConfirmationDialog GetDeleteCollectionDialog(string name)
      {
        DialogWrapper = new TestDialogWrapper(DeleteCollectionDialog.Result);
        var dialog = base.GetDeleteCollectionDialog(name);
        return dialog;
      }
      public override ConfirmationDialog GetDeleteListDialog(string name)
      {
        DialogWrapper = new TestDialogWrapper(DeleteListDialog.Result);
        var dialog = base.GetDeleteListDialog(name);
        return dialog;
      }
      public override ConfirmationDialog GetOverrideDialog(string name)
      {
        DialogWrapper = new TestDialogWrapper(OverrideDialog.Result);
        var dialog = base.GetOverrideDialog(name);
        return dialog;
      }
      public override ConfirmationDialog GetSaveUnsavedDialog()
      {
        DialogWrapper = new TestDialogWrapper(SaveUnsavedDialog.Result);
        var dialog = base.GetSaveUnsavedDialog();
        return dialog;
      }
      public override GridViewDialog GetCardPrintDialog(MTGCardViewModel[] printViewModels)
      {
        DialogWrapper = new TestDialogWrapper(CardPrintDialog.Result);
        var dialog = base.GetCardPrintDialog(printViewModels);
        dialog.Selection = CardPrintDialog.Values;
        return dialog;
      }
      public override ComboBoxDialog GetLoadDialog(string[] names)
      {
        DialogWrapper = new TestDialogWrapper(LoadDialog.Result);
        var dialog = base.GetLoadDialog(names);
        dialog.Selection = LoadDialog.Values;
        return dialog;
      }
      public override TextBoxDialog GetSaveDialog(string name)
      {
        DialogWrapper = new TestDialogWrapper(SaveDialog.Result);
        var dialog = base.GetSaveDialog(SaveDialog.Values);
        return dialog;
      }
    }

    #region List Tests
    [TestMethod]
    public async Task NewListTest()
    {
      var list = new MTGCardCollectionList() { Name = "First" };
      var vm = new CardCollectionsViewModel(new TestCardAPI(), new TestInMemoryMTGCardCollectionRepository(new TestCardAPI()))
      {
        Dialogs = new TestCardCollectionsDialogs()
        {
          NewCollectionListDialog = new() { Values = (list.Name, list.SearchQuery)},
        }
      };

      await vm.NewCollectionListDialog();
      Assert.AreEqual(1, vm.Collection.CollectionLists.Count);
      Assert.AreEqual(list.Name, vm.Collection.CollectionLists[0].Name);
      Assert.IsTrue(vm.HasUnsavedChanges);
    }

    [TestMethod]
    public async Task DeleteListTest()
    {
      var list = new MTGCardCollectionList() { Name = "First" };
      var vm = new CardCollectionsViewModel(new TestCardAPI(), new TestInMemoryMTGCardCollectionRepository(new TestCardAPI()))
      {
        Dialogs = new TestCardCollectionsDialogs()
        {
          NewCollectionListDialog = new() { Values = (list.Name, list.SearchQuery) },
          DeleteListDialog = new()
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
        Dialogs = new TestCardCollectionsDialogs()
        {
          NewCollectionListDialog = new() { Values = (list.Name, list.SearchQuery) },
          EditCollectionListDialog = new() { Values = (editedList.Name, editedList.SearchQuery) },
        }
      };

      await vm.NewCollectionListDialog();

      await vm.EditCollectionListDialog();
      Assert.AreEqual(1, vm.Collection.CollectionLists.Count);
      Assert.AreEqual(editedList.Name, vm.Collection.CollectionLists[0].Name);
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
        Dialogs = new TestCardCollectionsDialogs()
        {
          NewCollectionListDialog = new() { Values = (firstlist.Name, firstlist.SearchQuery) },
        }
      };

      await vm.NewCollectionListDialog();
      vm.Collection.CollectionLists.Add(secondlist);
      Assert.AreEqual(firstlist.Name, vm.SelectedList.Name);
      
      vm.ChangeSelectedCollectionList(secondlist);
      Assert.AreEqual(secondlist.Name, vm.SelectedList.Name);
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
        Dialogs = new TestCardCollectionsDialogs()
        {
          LoadDialog = new() { Values = collection.Name },
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
        Dialogs = new TestCardCollectionsDialogs()
        {
          NewCollectionListDialog = new() { Values = (list.Name, list.SearchQuery) },
          SaveDialog = new() { Values = collectionName }
        }
      };

      await vm.NewCollectionListDialog();

      await vm.SaveCollectionDialog();
      Assert.AreEqual(1, (await repo.Get()).ToList().Count);
      Assert.AreEqual(1, (await repo.Get(collectionName))?.CollectionLists.Count);
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
        Dialogs = new TestCardCollectionsDialogs()
        {
          SaveDialog = new() { Values = collectionName },
          DeleteCollectionDialog = new(),
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
