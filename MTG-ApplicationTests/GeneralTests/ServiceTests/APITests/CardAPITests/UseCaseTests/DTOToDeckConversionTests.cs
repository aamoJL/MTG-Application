using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplication.Features.DeckEditor.Editor.Services.Converters;
using MTGApplication.Features.DeckEditor.Models;
using MTGApplication.General.Models;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplicationTests.TestUtility.API;
using MTGApplicationTests.TestUtility.Mocker;

namespace MTGApplicationTests.GeneralTests.Services.APITests.CardAPITests.UseCaseTests;
[TestClass]
public class DTOToDeckConversionTests
{
  [TestMethod]
  public async Task Execute_DTOConvertedToDeck()
  {
    var cards = new DeckEditorMTGCard[]
    {
        DeckEditorMTGCardMocker.CreateMTGCardModel(name: "first"),
        DeckEditorMTGCardMocker.CreateMTGCardModel(name: "second"),
        DeckEditorMTGCardMocker.CreateMTGCardModel(name: "third"),
    };
    var deck = new DeckEditorMTGDeck()
    {
      Name = "First",
      DeckCards = new(cards),
    };

    var result = await new DTOToDeckEditorDeckConverter(new TestMTGCardImporter()
    {
      ExpectedCards = cards.Select(x => new CardImportResult.Card(x.Info, x.Count)).ToArray()
    }).Convert(DeckEditorMTGDeckToDTOConverter.Convert(deck));

    Assert.IsNotNull(result);
    Assert.AreEqual(3, result.DeckCards.Count);
    CollectionAssert.AreEquivalent(
      deck.DeckCards.Select(x => x.Info).ToArray(),
      result.DeckCards.Select(x => x.Info).ToArray());
  }
}
