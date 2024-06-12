using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Features.DeckEditor;
using MTGApplicationTests.TestUtility.API;
using MTGApplicationTests.TestUtility.Mocker;

namespace MTGApplicationTests.FeatureTests.DeckEditorTests.ViewModel.CommanderViewModelTests;
public partial class CommanderViewModelTests
{
  [TestClass]
  public class ChangeTests
  {
    [TestMethod]
    public async Task Change_ToCard_CardIsCard()
    {
      var card = DeckEditorMTGCardMocker.CreateMTGCardModel();
      var viewmodel = new CommanderViewModel(new TestMTGCardImporter());

      await viewmodel.ChangeCommanderCommand.ExecuteAsync(card);

      Assert.AreEqual(card.Info.Name, viewmodel.Card.Info.Name);
    }

    [TestMethod]
    public async Task Change_ToNull_CardIsNull()
    {
      var viewmodel = new CommanderViewModel(new TestMTGCardImporter())
      {
        Card = DeckEditorMTGCardMocker.CreateMTGCardModel()
      };

      await viewmodel.ChangeCommanderCommand.ExecuteAsync(null);

      Assert.IsNull(viewmodel.Card);
    }
  }
}
