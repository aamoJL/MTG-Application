using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Features.DeckEditor.Commanders.ViewModels;
using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplicationTests.TestUtility.Mocker;
using static MTGApplicationTests.FeatureTests.DeckEditorTests.ViewModel.DeckEditorViewModelTests.DeckEditorViewModelTests;

namespace MTGApplicationTests.FeatureTests.DeckEditorTests.ViewModel.CommanderViewModelTests;
public partial class CommanderViewModelTests
{
  [TestClass]
  public class ChangeTests : DeckEditorViewModelTestsBase
  {
    [TestMethod]
    public async Task Change_ToCard_InvokedWithCard()
    {
      DeckEditorMTGCard result = null;

      var viewmodel = new CommanderViewModel(_dependencies.Importer)
      {
        OnChange = (card) => { result = card; }
      };
      var card = DeckEditorMTGCardMocker.CreateMTGCardModel();

      await viewmodel.ChangeCommanderCommand.ExecuteAsync(card);

      Assert.AreEqual(card.Info.Name, result.Info.Name);
    }

    [TestMethod]
    public async Task Change_ToNull_InvokedWithNull()
    {
      DeckEditorMTGCard result = null;

      var viewmodel = new CommanderViewModel(_dependencies.Importer)
      {
        OnChange = (card) => { result = card; }
      };

      await viewmodel.ChangeCommanderCommand.ExecuteAsync(null);

      Assert.IsNull(result);
    }
  }
}
