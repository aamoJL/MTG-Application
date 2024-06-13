using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Features.DeckEditor;
using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplicationTests.TestUtility.API;
using MTGApplicationTests.TestUtility.Mocker;

namespace MTGApplicationTests.FeatureTests.DeckEditorTests.ViewModel.CommanderViewModelTests;
public partial class CommanderViewModelTests
{
  [TestClass]
  public class ChangeTests
  {
    [TestMethod]
    public async Task Change_ToCard_InvokedWithCard()
    {
      var card = DeckEditorMTGCardMocker.CreateMTGCardModel();
      DeckEditorMTGCard? result = null;
      var viewmodel = new CommanderViewModel(new TestMTGCardImporter(), () => null)
      {
        OnChange = (card) => { result = card; }
      };

      await viewmodel.ChangeCommanderCommand.ExecuteAsync(card);

      Assert.AreEqual(card.Info.Name, result?.Info.Name);
    }

    [TestMethod]
    public async Task Change_ToNull_InvokedWithNull()
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
