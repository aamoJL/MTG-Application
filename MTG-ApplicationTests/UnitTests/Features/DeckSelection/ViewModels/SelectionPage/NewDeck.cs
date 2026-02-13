using MTGApplication.General.Services.NotificationService;
using MTGApplicationTests.TestUtility.Services;

namespace MTGApplicationTests.UnitTests.Features.DeckSelection.ViewModels.SelectionPage;

[TestClass]
public class NewDeck
{
  [TestMethod]
  public void New_DeckSelected()
  {
    string selected = null;
    var factory = new TestDeckSelectionPageViewModelFactory()
    {
      OnDeckSelected = deck => selected = deck.Name
    };
    var vm = factory.Build();

    vm.NewDeckCommand.Execute(null);

    Assert.AreEqual(string.Empty, selected);
  }

  [TestMethod]
  public void New_Exception_NotificationShown()
  {
    var factory = new TestDeckSelectionPageViewModelFactory()
    {
      OnDeckSelected = deck => throw new()
    };
    var vm = factory.Build();

    vm.NewDeckCommand.Execute(null);

    NotificationAssert.NotificationSent(NotificationService.NotificationType.Error, factory.Notifier);
  }
}
