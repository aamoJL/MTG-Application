using MTGApplication.Features.DeckEditor.ViewModels;
using MTGApplicationTests.TestUtility.Importers;

namespace MTGApplicationTests.UnitTests.Features.DeckEditor.CardList.UseCases.GroupedList;

[TestClass]
public class ExportCardsFromGroup
{
  [TestMethod]
  public async Task Export()
  {
    var viewmodel = new CardGroupViewModel(string.Empty, [], new TestMTGCardImporter());

    await Assert.ThrowsAsync<NotImplementedException>(() => viewmodel.ExportCardsCommand.ExecuteAsync(null));
  }
}
