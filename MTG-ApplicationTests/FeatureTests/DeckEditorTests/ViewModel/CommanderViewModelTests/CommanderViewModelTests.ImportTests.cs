using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Features.DeckEditor;
using MTGApplication.General.Services.IOService;
using MTGApplicationTests.API;
using MTGApplicationTests.Services;

namespace MTGApplicationTests.FeatureTests.DeckEditorTests.ViewModel.CommanderViewModelTests;
public partial class CommanderViewModelTests
{
  [TestClass]
  public class ImportTests
  {
    [TestMethod]
    public void Change_ToNull_CardIsNull()
    {
      var card = Mocker.MTGCardModelMocker.CreateMTGCardModel();
      var viewmodel = new CommanderViewModel(new TestCardAPI());

      JsonService.TrySerializeObject(card, out var json);

      viewmodel.ImportCommand.Execute(json);

      Assert.AreEqual(card.Info.Name, viewmodel.Card.Info.Name);
    }
  }
}
