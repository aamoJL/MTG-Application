using MTGApplication.Features.DeckSelection.Models;
using MTGApplication.General.Services.NotificationService;
using MTGApplicationTests.TestUtility.Services;

namespace MTGApplicationTests.UnitTests.Features.DeckSelection.ViewModels.SelectionPage;

[TestClass]
public class SelectDeck
{
  [TestMethod]
  public void Select_DeckSelected()
  {
    string? selected = null;
    var factory = new TestDeckSelectionPageViewModelFactory()
    {
      OnDeckSelected = deck => selected = deck.Name
    };
    var vm = factory.Build();

    vm.SelectDeckCommand.Execute(new DeckSelectionDeck() { Name = "Deck" });

    Assert.AreEqual("Deck", selected);
  }

  [TestMethod]
  public void Select_Exception_NotificationShown()
  {
    var factory = new TestDeckSelectionPageViewModelFactory()
    {
      Notifier = new() { ThrowOnError = false },
      OnDeckSelected = deck => throw new()
    };
    var vm = factory.Build();

    vm.SelectDeckCommand.Execute(null);

    NotificationAssert.NotificationSent(NotificationService.NotificationType.Error, factory.Notifier);
  }
}
