using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Features.DeckTesting.Models;
using MTGApplication.Features.DeckTesting.ViewModels;
using MTGApplicationTests.TestUtility.Mocker;
using MTGApplicationTests.TestUtility.ViewModel.TestInterfaces;

namespace MTGApplicationTests.FeatureTests.DeckTestingTests;
public partial class DeckTestingPageViewModelTests
{
  [TestClass]
  public class DrawCardTests : ICanExecuteCommandTests
  {
    [TestMethod("Should not be able to execute if the library has no cards")]
    public void InvalidState_CanNotExecute()
    {
      var deck = new DeckTestingDeck(
        DeckCards: [],
        Commander: null,
        Partner: null);
      var viewmodel = new DeckTestingPageViewModel(deck);

      viewmodel.Library.Clear();

      Assert.IsFalse(viewmodel.DrawCardCommand.CanExecute(null));
    }

    [TestMethod("Should be able to execute if the library has cards")]
    public void ValidState_CanExecute()
    {
      var deck = new DeckTestingDeck(
        DeckCards: [.. MTGCardMocker.Mock(count: 10)],
        Commander: null,
        Partner: null);
      var viewmodel = new DeckTestingPageViewModel(deck);

      viewmodel.Library.Add(new DeckTestingMTGCard(MTGCardInfoMocker.MockInfo()));

      Assert.IsTrue(viewmodel.DrawCardCommand.CanExecute(null));
    }

    [TestMethod]
    public void DrawCard_CardRemovedFromLibrary()
    {
      var deck = new DeckTestingDeck(
        DeckCards: [.. MTGCardMocker.Mock(count: 10)],
        Commander: null,
        Partner: null);
      var viewmodel = new DeckTestingPageViewModel(deck);

      viewmodel.Library.Add(new DeckTestingMTGCard(MTGCardInfoMocker.MockInfo()));

      Assert.AreEqual(1, viewmodel.Library.Count);

      viewmodel.DrawCardCommand.Execute(null);

      Assert.AreEqual(0, viewmodel.Library.Count);
    }

    [TestMethod]
    public void DrawCard_CardAddedToHand()
    {
      var deck = new DeckTestingDeck(
        DeckCards: [.. MTGCardMocker.Mock(count: 10)],
        Commander: null,
        Partner: null);
      var viewmodel = new DeckTestingPageViewModel(deck);

      viewmodel.Library.Add(new DeckTestingMTGCard(MTGCardInfoMocker.MockInfo()));

      Assert.AreEqual(1, viewmodel.Library.Count);
      Assert.AreEqual(0, viewmodel.Hand.Count);

      viewmodel.DrawCardCommand.Execute(null);

      Assert.AreEqual(1, viewmodel.Hand.Count);
    }
  }
}
