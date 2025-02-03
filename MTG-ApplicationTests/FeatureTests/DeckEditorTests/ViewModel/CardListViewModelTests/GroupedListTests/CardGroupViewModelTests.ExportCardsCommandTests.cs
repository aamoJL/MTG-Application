using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Features.DeckEditor.ViewModels;
using MTGApplicationTests.TestUtility.Importers;

namespace MTGApplicationTests.FeatureTests.DeckEditorTests.ViewModel.CardListViewModelTests.GroupedListTests;

public partial class CardGroupViewModelTests
{
  [TestClass]
  public class ExportCardsCommandTests
  {
    [TestMethod]
    public async Task Export()
    {
      var viewmodel = new CardGroupViewModel(string.Empty, [], new TestMTGCardImporter());

      await Assert.ThrowsExceptionAsync<NotImplementedException>(() => viewmodel.ExportCardsCommand.ExecuteAsync(null));
    }
  }
}
