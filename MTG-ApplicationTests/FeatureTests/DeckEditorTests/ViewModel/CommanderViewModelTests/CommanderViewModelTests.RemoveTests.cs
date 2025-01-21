using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Features.DeckEditor.Commanders.ViewModels;
using static MTGApplicationTests.FeatureTests.DeckEditorTests.ViewModel.DeckEditorViewModelTests.DeckEditorViewModelTests;

namespace MTGApplicationTests.FeatureTests.DeckEditorTests.ViewModel.CommanderViewModelTests;
public partial class CommanderViewModelTests
{
  [TestClass]
  public class RemoveTests : DeckEditorViewModelTestsBase
  {
    [TestMethod]
    public void Remove_InvokedWithNull()
    {
      var result = _savedDeck.Commander;

      var viewmodel = new CommanderViewModel(_dependencies.Importer)
      {
        Card = _savedDeck.Commander,
        OnChange = (card) => { result = card; }
      };

      viewmodel.RemoveCommanderCommand.Execute(null);

      Assert.IsNull(result);
    }
  }
}
