using MTGApplication.Features.DeckEditor.CardList.Services.Factories;
using MTGApplication.Features.DeckEditor.ViewModels;
using MTGApplicationTests.TestUtility.Importers;
using MTGApplicationTests.TestUtility.Mocker;
using MTGApplicationTests.TestUtility.Services;

namespace MTGApplicationTests.FeatureTests.DeckEditorTests.ViewModel.CardListViewModelTests.GroupedListTests;

public partial class GroupedCardListViewModelTest
{
  [TestClass]
  public class RemoveCardGroupTests
  {
    [TestMethod]
    public void RemoveCardGroup_EmptyOrNullKey_CanNotExecute()
    {
      var viewmodel = new GroupedCardListViewModel([], importer: new TestMTGCardImporter());

      Assert.IsFalse(viewmodel.RemoveGroupCommand.CanExecute(new CardGroupViewModel(string.Empty, [], new TestMTGCardImporter())));
      Assert.IsFalse(viewmodel.RemoveGroupCommand.CanExecute(null));
    }

    [TestMethod]
    public void RemoveCardGroup_HasKey_CanExecute()
    {
      var viewmodel = new GroupedCardListViewModel([], importer: new TestMTGCardImporter());

      Assert.IsTrue(viewmodel.RemoveGroupCommand.CanExecute(new CardGroupViewModel("key", [], new TestMTGCardImporter())));
    }

    [TestMethod]
    public void RemoveCardGroup_Exists_GroupRemoved()
    {
      var name = "New Group";
      var viewmodel = new GroupedCardListViewModel([], importer: new TestMTGCardImporter());

      var group = new GroupedCardListCardGroupFactory(viewmodel).CreateCardGroup(name);
      viewmodel.Groups.Add(group);

      viewmodel.RemoveGroupCommand.Execute(group);

      Assert.HasCount(1, viewmodel.Groups);
      Assert.IsFalse(viewmodel.Groups.Any(x => x.Key == name));
    }

    [TestMethod]
    public void RemoveCardGroup_Exists_SuccessNotificationSent()
    {
      var name = "New Group";
      var notifier = new TestNotifier();
      var viewmodel = new GroupedCardListViewModel([], importer: new TestMTGCardImporter())
      {
        Notifier = notifier
      };

      var group = new GroupedCardListCardGroupFactory(viewmodel).CreateCardGroup(name);
      viewmodel.Groups.Add(group);

      viewmodel.RemoveGroupCommand.Execute(group);

      NotificationAssert.NotificationSent(MTGApplication.General.Services.NotificationService.NotificationService.NotificationType.Success, notifier);
    }

    [TestMethod]
    public void RemoveCardGroup_DoesNotExist_NoChanges()
    {
      var name = "New Group";
      var viewmodel = new GroupedCardListViewModel([], importer: new TestMTGCardImporter());

      viewmodel.Groups.Add(new GroupedCardListCardGroupFactory(viewmodel).CreateCardGroup(name));

      var initCount = viewmodel.Groups.Count;

      viewmodel.RemoveGroupCommand.Execute(new CardGroupViewModel(string.Empty, [], new TestMTGCardImporter()));

      Assert.AreEqual(initCount, viewmodel.Groups.Count);
      Assert.IsTrue(viewmodel.Groups.Any(x => x.Key == name));
    }

    [TestMethod]
    public void RemoveCardGroup_DoesNotExist_ErrorNotificationSent()
    {
      var notifier = new TestNotifier();
      var viewmodel = new GroupedCardListViewModel([], importer: new TestMTGCardImporter())
      {
        Notifier = notifier
      };

      viewmodel.RemoveGroupCommand.Execute(new CardGroupViewModel("key", [], new TestMTGCardImporter()));

      NotificationAssert.NotificationSent(MTGApplication.General.Services.NotificationService.NotificationService.NotificationType.Error, notifier);
    }

    [TestMethod]
    public void RemoveCardGroup_Success_CardsMovedToDefault()
    {
      var name = "New Group";
      var viewmodel = new GroupedCardListViewModel(
        cards: [
          DeckEditorMTGCardMocker.CreateMTGCardModel(name: "A", group: name),
          DeckEditorMTGCardMocker.CreateMTGCardModel(name: "B", group: name),
          DeckEditorMTGCardMocker.CreateMTGCardModel(name: "C", group: name),
        ],
        importer: new TestMTGCardImporter());

      var initCount = viewmodel.Cards.Count;

      Assert.AreEqual(0, viewmodel.Groups.FirstOrDefault(x => x.Key == string.Empty).Count);

      var group = viewmodel.Groups.First(x => x.Key == name);
      viewmodel.RemoveGroupCommand.Execute(group);

      Assert.AreEqual(initCount, viewmodel.Groups.FirstOrDefault(x => x.Key == string.Empty).Count);
    }
  }
}
