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
      var viewmodel = new GroupedCardListViewModel(
        importer: new TestMTGCardImporter(),
        groupBy: x => x.Group,
        confirmers: new()
        {
          AddCardGroupConfirmer = new TestExceptionConfirmer<string>()
        });

      await ConfirmationAssert.ConfirmationShown(() => viewmodel.AddCardGroupCommand.ExecuteAsync(null));
    }

    [TestMethod]
    public async Task AddCardGroup_WithParameter_NoConfirmationShown()
    {
      var viewmodel = new GroupedCardListViewModel(
        importer: new TestMTGCardImporter(),
        groupBy: x => x.Group,
        confirmers: new()
        {
          AddCardGroupConfirmer = new TestExceptionConfirmer<string>()
        });

      await viewmodel.AddCardGroupCommand.ExecuteAsync("New group");
    }

    [TestMethod]
    public async Task AddCardGroup_WithoutParameter_New_GroupAdded()
    {
      var viewmodel = new GroupedCardListViewModel(
        importer: new TestMTGCardImporter(),
        groupBy: x => x.Group,
        confirmers: new()
        {
          AddCardGroupConfirmer = new() { OnConfirm = async arg => await Task.FromResult("New group") }
        });
      var initCount = viewmodel.Groups.Count;

      await viewmodel.AddCardGroupCommand.ExecuteAsync(null);

      Assert.AreEqual(initCount + 1, viewmodel.Groups.Count);
    }

    [TestMethod]
    public async Task AddCardGroup_WithParameter_New_GroupAdded()
    {
      var viewmodel = new GroupedCardListViewModel(
        importer: new TestMTGCardImporter(),
        groupBy: x => x.Group);
      var initCount = viewmodel.Groups.Count;

      await viewmodel.AddCardGroupCommand.ExecuteAsync("New group");

      Assert.AreEqual(initCount + 1, viewmodel.Groups.Count);
    }

    [TestMethod]
    public async Task AddCardGroup_WithoutParameter_Existing_GroupNotAdded()
    {
      var viewmodel = new GroupedCardListViewModel(
        importer: new TestMTGCardImporter(),
        groupBy: x => x.Group,
        confirmers: new()
        {
          AddCardGroupConfirmer = new() { OnConfirm = async arg => await Task.FromResult(string.Empty) }
        });
      var initCount = viewmodel.Groups.Count;

      await viewmodel.AddCardGroupCommand.ExecuteAsync(null);

      Assert.AreEqual(initCount, viewmodel.Groups.Count);
    }

    [TestMethod]
    public async Task AddCardGroup_WithParameter_Existing_GroupNotAdded()
    {
      var viewmodel = new GroupedCardListViewModel(
        importer: new TestMTGCardImporter(),
        groupBy: x => x.Group);
      var initCount = viewmodel.Groups.Count;

      await viewmodel.AddCardGroupCommand.ExecuteAsync(string.Empty);

      Assert.AreEqual(initCount, viewmodel.Groups.Count);
    }

    [TestMethod]
    public async Task AddCardGroup_Existing_ErrorNotificationSent()
    {
      var viewmodel = new GroupedCardListViewModel(
        importer: new TestMTGCardImporter(),
        groupBy: x => x.Group,
        confirmers: new()
        {
          AddCardGroupConfirmer = new() { OnConfirm = async arg => await Task.FromResult(string.Empty) }
        })
      {
        Notifier = new()
        {
          OnNotify = arg => throw new NotificationException(arg)
        }
      };

      await NotificationAssert.NotificationSent(NotificationType.Error,
        () => viewmodel.AddCardGroupCommand.ExecuteAsync(null));
    }

    [TestMethod]
    public async Task AddCardGroup_New_SuccessNotificationSent()
    {
      var viewmodel = new GroupedCardListViewModel(
        importer: new TestMTGCardImporter(),
        groupBy: x => x.Group,
        confirmers: new()
        {
          AddCardGroupConfirmer = new() { OnConfirm = async arg => await Task.FromResult("New group") }
        })
      {
        Notifier = new()
        {
          OnNotify = arg => throw new NotificationException(arg)
        }
      };

      await NotificationAssert.NotificationSent(NotificationType.Success,
        () => viewmodel.AddCardGroupCommand.ExecuteAsync(null));
    }

    [TestMethod]
    public async Task AddCardGroup_Success_GroupsInAlphabeticalOrder()
    {
      var viewmodel = new GroupedCardListViewModel(
        importer: new TestMTGCardImporter(),
        groupBy: x => x.Group);

      await viewmodel.AddCardGroupCommand.ExecuteAsync("A");
      await viewmodel.AddCardGroupCommand.ExecuteAsync("D");
      await viewmodel.AddCardGroupCommand.ExecuteAsync("B");
      await viewmodel.AddCardGroupCommand.ExecuteAsync("C");

      var expected = new string[] { "A", "B", "C", "D", string.Empty };

      CollectionAssert.AreEqual(expected, viewmodel.Groups.Select(x => x.Key).ToArray());
    }
  }
}
