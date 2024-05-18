using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Features.DeckEditor;
using MTGApplicationTests.API;
using MTGApplicationTests.Services;

namespace MTGApplicationTests.FeatureTests.DeckEditorTests.ViewModel.CommanderViewModelTests;
public partial class CommanderViewModelTests
{
  [TestClass]
  public class ChangeTests
  {
    [TestMethod]
    public void Change_ToCard_CardIsCard()
    {
      var card = Mocker.MTGCardModelMocker.CreateMTGCardModel();
      var viewmodel = new CommanderViewModel(new TestCardAPI());

      viewmodel.ChangeCommand.Execute(card);

      Assert.AreEqual(card.Info.Name, viewmodel.Card.Info.Name);
    }

    [TestMethod]
    public void Change_ToNull_CardIsNull()
    {
      var viewmodel = new CommanderViewModel(new TestCardAPI())
      {
        Card = Mocker.MTGCardModelMocker.CreateMTGCardModel()
      };

      viewmodel.ChangeCommand.Execute(null);

      Assert.IsNull(viewmodel.Card);
    }
  }
}
