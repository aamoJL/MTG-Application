using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Features.DeckEditor.Commanders.ViewModels;
using MTGApplication.Features.DeckEditor.Editor.Models;
using static MTGApplicationTests.FeatureTests.DeckEditorTests.ViewModel.DeckEditorViewModelTests.DeckEditorViewModelTests;

namespace MTGApplicationTests.FeatureTests.DeckEditorTests.ViewModel.CommanderViewModelTests;
public partial class CommanderCommandsTests
{
  [TestClass]
  public class RemoveTests : DeckEditorViewModelTestsBase
  {
    [TestMethod]
    public async Task Remove_InvokedWithNull()
    {
      DeckEditorMTGCard result = null;

      var viewmodel = new CommanderCommands(new Mocker(_dependencies).MockVM(), CommanderCommands.CommanderType.Commander)
      {
        OnChange = (card) => { result = card; }
      };

      await viewmodel.ChangeCommanderCommand.ExecuteAsync(null);

      Assert.IsNull(result);
    }
  }
}
