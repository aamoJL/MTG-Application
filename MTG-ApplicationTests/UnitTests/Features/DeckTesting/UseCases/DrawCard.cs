using MTGApplication.Features.DeckTesting.Models;
using MTGApplication.Features.DeckTesting.ViewModels;
using MTGApplicationTests.TestUtility.Importers;
using MTGApplicationTests.TestUtility.Mocker;
using MTGApplicationTests.TestUtility.ViewModel.TestInterfaces;

namespace MTGApplicationTests.UnitTests.Features.DeckTesting.UseCases;

[TestClass]
public class DrawCard : ICanExecuteCommandTests
{
  private readonly TestMTGCardImporter _importer = new();

  [TestMethod(DisplayName = "Should not be able to execute if the library has no cards")]
  public void InvalidState_CanNotExecute()
  {
    var deck = new DeckTestingDeck(
      DeckCards: [],
      Commander: null,
      Partner: null);
    var viewmodel = new DeckTestingPageViewModel(deck, _importer);

    viewmodel.Library.Clear();

    Assert.IsFalse(viewmodel.DrawCardCommand.CanExecute(null));
  }

  [TestMethod(DisplayName = "Should be able to execute if the library has cards")]
  public void ValidState_CanExecute()
  {
    var deck = new DeckTestingDeck(
      DeckCards: [.. MTGCardMocker.Mock(count: 10)],
      Commander: null,
      Partner: null);
    var viewmodel = new DeckTestingPageViewModel(deck, _importer);

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
    var viewmodel = new DeckTestingPageViewModel(deck, _importer);

    viewmodel.Library.Add(new DeckTestingMTGCard(MTGCardInfoMocker.MockInfo()));

    Assert.HasCount(1, viewmodel.Library);

    viewmodel.DrawCardCommand.Execute(null);

    Assert.IsEmpty(viewmodel.Library);
  }

  [TestMethod]
  public void DrawCard_CardAddedToHand()
  {
    var deck = new DeckTestingDeck(
      DeckCards: [.. MTGCardMocker.Mock(count: 10)],
      Commander: null,
      Partner: null);
    var viewmodel = new DeckTestingPageViewModel(deck, _importer);

    viewmodel.Library.Add(new DeckTestingMTGCard(MTGCardInfoMocker.MockInfo()));

    Assert.HasCount(1, viewmodel.Library);
    Assert.IsEmpty(viewmodel.Hand);

    viewmodel.DrawCardCommand.Execute(null);

    Assert.HasCount(1, viewmodel.Hand);
  }
}
