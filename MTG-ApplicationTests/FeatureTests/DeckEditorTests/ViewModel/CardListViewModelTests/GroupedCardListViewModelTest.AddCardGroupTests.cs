using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Features.DeckEditor.ViewModels;
using MTGApplicationTests.TestUtility.Importers;
using MTGApplicationTests.TestUtility.Services;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplicationTests.FeatureTests.DeckEditorTests.ViewModel.CardListViewModelTests;

public partial class GroupedCardListViewModelTest
{
  [TestClass]
  public class AddCardGroupTests
  {
    [TestMethod]
    public async Task AddCardGroup_WithoutParameter_ConfirmationShown()
    {
      var confirmer = new TestConfirmer<string>();
      var viewmodel = new GroupedCardListViewModel(
        importer: new TestMTGCardImporter(),
        confirmers: new()
        {
          AddCardGroupConfirmer = confirmer
        });

      await viewmodel.AddGroupCommand.ExecuteAsync(null);

      ConfirmationAssert.ConfirmationShown(confirmer);
    }

    [TestMethod]
    public async Task AddCardGroup_WithParameter_NoConfirmationShown()
    {
      var viewmodel = new GroupedCardListViewModel(
        importer: new TestMTGCardImporter(),
        confirmers: new()
        {
          AddCardGroupConfirmer = new TestConfirmer<string>()
        });

      await viewmodel.AddGroupCommand.ExecuteAsync("New group");
    }

    [TestMethod]
    public async Task AddCardGroup_WithoutParameter_New_GroupAdded()
    {
      var name = "New group";
      var viewmodel = new GroupedCardListViewModel(
        importer: new TestMTGCardImporter(),
        confirmers: new()
        {
          AddCardGroupConfirmer = new() { OnConfirm = async arg => await Task.FromResult(name) }
        });
      var initCount = viewmodel.Groups.Count;

      await viewmodel.AddGroupCommand.ExecuteAsync(null);

      Assert.AreEqual(initCount + 1, viewmodel.Groups.Count);
      Assert.IsTrue(viewmodel.Groups.Any(x => x.Key == name));
    }

    [TestMethod]
    public async Task AddCardGroup_Cancel_GroupNotAdded()
    {
      var name = "New group";
      var viewmodel = new GroupedCardListViewModel(
        importer: new TestMTGCardImporter(),
        confirmers: new()
        {
          AddCardGroupConfirmer = new() { OnConfirm = async arg => await Task.FromResult<string>(null) }
        });
      var initCount = viewmodel.Groups.Count;

      await viewmodel.AddGroupCommand.ExecuteAsync(null);

      Assert.AreEqual(initCount, viewmodel.Groups.Count);
      Assert.IsFalse(viewmodel.Groups.Any(x => x.Key == name));
    }

    [TestMethod]
    public async Task AddCardGroup_WithParameter_New_GroupAdded()
    {
      var name = "New group";
      var viewmodel = new GroupedCardListViewModel(
        importer: new TestMTGCardImporter());
      var initCount = viewmodel.Groups.Count;

      await viewmodel.AddGroupCommand.ExecuteAsync(name);

      Assert.AreEqual(initCount + 1, viewmodel.Groups.Count);
      Assert.IsTrue(viewmodel.Groups.Any(x => x.Key == name));
    }

    [TestMethod]
    public async Task AddCardGroup_WithoutParameter_Existing_GroupNotAdded()
    {
      var name = "New Group";
      var viewmodel = new GroupedCardListViewModel(
        importer: new TestMTGCardImporter(),
        confirmers: new()
        {
          AddCardGroupConfirmer = new() { OnConfirm = async arg => await Task.FromResult(name) }
        });

      await viewmodel.AddGroupCommand.ExecuteAsync(name);
      var initCount = viewmodel.Groups.Count;

      await viewmodel.AddGroupCommand.ExecuteAsync(null);

      Assert.AreEqual(initCount, viewmodel.Groups.Count);
    }

    [TestMethod]
    public async Task AddCardGroup_WithParameter_Existing_GroupNotAdded()
    {
      var name = "New Group";
      var viewmodel = new GroupedCardListViewModel(
        importer: new TestMTGCardImporter());

      await viewmodel.AddGroupCommand.ExecuteAsync(name);
      var initCount = viewmodel.Groups.Count;

      await viewmodel.AddGroupCommand.ExecuteAsync(name);

      Assert.AreEqual(initCount, viewmodel.Groups.Count);
    }

    [TestMethod]
    public async Task AddCardGroup_Existing_ErrorNotificationSent()
    {
      var name = "New Group";
      var notifier = new TestNotifier();
      var viewmodel = new GroupedCardListViewModel(
        importer: new TestMTGCardImporter(),
        confirmers: new()
        {
          AddCardGroupConfirmer = new() { OnConfirm = async arg => await Task.FromResult(name) }
        })
      {
        Notifier = notifier
      };

      await viewmodel.AddGroupCommand.ExecuteAsync(name);

      NotificationAssert.NotificationSent(NotificationType.Success, notifier);

      await viewmodel.AddGroupCommand.ExecuteAsync(null);

      NotificationAssert.NotificationSent(NotificationType.Error, notifier);
    }

    [TestMethod]
    public async Task AddCardGroup_New_SuccessNotificationSent()
    {
      var notifier = new TestNotifier();
      var viewmodel = new GroupedCardListViewModel(
        importer: new TestMTGCardImporter(),
        confirmers: new()
        {
          AddCardGroupConfirmer = new() { OnConfirm = async arg => await Task.FromResult("New group") }
        })
      {
        Notifier = notifier
      };

      await viewmodel.AddGroupCommand.ExecuteAsync(null);

      NotificationAssert.NotificationSent(NotificationType.Success, notifier);
    }

    [TestMethod]
    public async Task AddCardGroup_Success_GroupsInAlphabeticalOrder()
    {
      var viewmodel = new GroupedCardListViewModel(
        importer: new TestMTGCardImporter());

      await viewmodel.AddGroupCommand.ExecuteAsync("A");
      await viewmodel.AddGroupCommand.ExecuteAsync("D");
      await viewmodel.AddGroupCommand.ExecuteAsync("B");
      await viewmodel.AddGroupCommand.ExecuteAsync("C");

      var expected = new string[] { "A", "B", "C", "D", string.Empty };

      CollectionAssert.AreEqual(expected, viewmodel.Groups.Select(x => x.Key).ToArray());
    }
  }
}
