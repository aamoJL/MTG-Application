using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Features.DeckEditor;
using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplicationTests.TestUtility.API;

namespace MTGApplicationTests.FeatureTests.DeckEditorTests.ViewModel.CommanderViewModelTests;
public partial class CommanderViewModelTests
{
  [TestClass]
  public class RemoveTests
  {
    [TestMethod]
    public async Task Remove_InvokedWithNull()
    {
      DeckEditorMTGCard? result = null;

      var viewmodel = new CommanderViewModel(new TestMTGCardImporter(), () => null)
      {
        OnChange = (card) => { result = card; }
      };

      await viewmodel.ChangeCommanderCommand.ExecuteAsync(null);

      Assert.IsNull(result);
    }
  }
}
