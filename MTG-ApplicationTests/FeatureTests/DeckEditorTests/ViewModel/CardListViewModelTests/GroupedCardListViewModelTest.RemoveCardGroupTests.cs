using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Features.DeckEditor.ViewModels;
using MTGApplicationTests.TestUtility.Importers;
using MTGApplicationTests.TestUtility.Mocker;
using MTGApplicationTests.TestUtility.Services;

namespace MTGApplicationTests.FeatureTests.DeckEditorTests.ViewModel.CardListViewModelTests;

public partial class GroupedCardListViewModelTest
{
  [TestClass]
  public class RemoveCardGroupTests
  {
    [TestMethod]
    public void RemoveCardGroup_EmptyOrNullKey_CanNotExecute()
    {
      var viewmodel = new GroupedCardListViewModel(
        importer: new TestMTGCardImporter());

      Assert.IsFalse(viewmodel.RemoveGroupCommand.CanExecute(string.Empty));
      Assert.IsFalse(viewmodel.RemoveGroupCommand.CanExecute(null));
    }

    [TestMethod]
    public void RemoveCardGroup_HasKey_CanExecute()
    {
      var viewmodel = new GroupedCardListViewModel(
        importer: new TestMTGCardImporter());

      Assert.IsTrue(viewmodel.RemoveGroupCommand.CanExecute("New Group"));
    }

    [TestMethod]
    public void RemoveCardGroup_Exists_GroupRemoved()
    {
      var name = "New Group";
      var viewmodel = new GroupedCardListViewModel(
        importer: new TestMTGCardImporter());

      viewmodel.Groups.Add(new(name, viewmodel));

      viewmodel.RemoveGroupCommand.Execute(name);

      Assert.AreEqual(1, viewmodel.Groups.Count);
      Assert.IsFalse(viewmodel.Groups.Any(x => x.Key == name));
    }

    [TestMethod]
    public void RemoveCardGroup_Exists_SuccessNotificationSent()
    {
      var name = "New Group";
      var notifier = new TestNotifier();
      var viewmodel = new GroupedCardListViewModel(
        importer: new TestMTGCardImporter())
      {
        Notifier = notifier
      };

      viewmodel.Groups.Add(new(name, viewmodel));

      viewmodel.RemoveGroupCommand.Execute(name);

      NotificationAssert.NotificationSent(MTGApplication.General.Services.NotificationService.NotificationService.NotificationType.Success, notifier);
    }

    [TestMethod]
    public void RemoveCardGroup_DoesNotExist_NoChanges()
    {
      var name = "New Group";
      var viewmodel = new GroupedCardListViewModel(
        importer: new TestMTGCardImporter());

      viewmodel.Groups.Add(new(name, viewmodel));

      var initCount = viewmodel.Groups.Count;

      viewmodel.RemoveGroupCommand.Execute("Nonexistent");

      Assert.AreEqual(initCount, viewmodel.Groups.Count);
      Assert.IsTrue(viewmodel.Groups.Any(x => x.Key == name));
    }

    [TestMethod]
    public void RemoveCardGroup_DoesNotExist_ErrorNotificationSent()
    {
      var notifier = new TestNotifier();
      var viewmodel = new GroupedCardListViewModel(
        importer: new TestMTGCardImporter())
      {
        Notifier = notifier
      };

      viewmodel.RemoveGroupCommand.Execute("New group");

      NotificationAssert.NotificationSent(MTGApplication.General.Services.NotificationService.NotificationService.NotificationType.Error, notifier);
    }

    [TestMethod]
    public void RemoveCardGroup_Success_CardsMovedToDefault()
    {
      var name = "New Group";
      var viewmodel = new GroupedCardListViewModel(
        importer: new TestMTGCardImporter())
      {
        Cards = [
          DeckEditorMTGCardMocker.CreateMTGCardModel(name: "A", group: name),
          DeckEditorMTGCardMocker.CreateMTGCardModel(name: "B", group: name),
          DeckEditorMTGCardMocker.CreateMTGCardModel(name: "C", group: name),
          ]
      };

      var initCount = viewmodel.Cards.Count;

      Assert.AreEqual(0, viewmodel.Groups.FirstOrDefault(x => x.Key == string.Empty)?.Count);

      viewmodel.RemoveGroupCommand.Execute(name);

      Assert.AreEqual(initCount, viewmodel.Groups.FirstOrDefault(x => x.Key == string.Empty)?.Count);
    }
  }
}
