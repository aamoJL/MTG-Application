using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Features.DeckEditor;
using MTGApplicationTests.API;
using MTGApplicationTests.Services;

namespace MTGApplicationTests.FeatureTests.DeckEditorTests.ViewModel.CommanderViewModelTests;
public partial class CommanderViewModelTests
{
  [TestClass]
  public class RemoveTests
  {
    [TestMethod]
    public async Task Remove_CardIsNull()
    {
      var viewmodel = new CommanderViewModel(new TestCardAPI())
      {
        Card = Mocker.MTGCardModelMocker.CreateMTGCardModel()
      };

      await viewmodel.ChangeCommand.ExecuteAsync(null);

      Assert.IsNull(viewmodel.Card);
    }
  }
}
