using MTGApplication.Features.CardCollectionEditor.CardCollectionList.Models;
using MTGApplicationTests.TestUtility.Mocker;
using MTGApplicationTests.UnitTests.Features.CardCollectionEditor.CardCollectionList.ViewModels;

namespace MTGApplicationTests.UnitTests.Features.CardCollectionEditor.CardCollectionList.UseCases;

[TestClass]
public class SwitchCardOwnership : CardCollectionListViewModelTestBase
{
  [TestMethod(DisplayName = "Should be able to execute if the given card is not null")]
  public async Task ValidParameter_CanExecute()
  {
    var card = new CardCollectionMTGCard(MTGCardInfoMocker.MockInfo());
    var viewmodel = await new Mocker(_dependencies)
    {
      Model = _savedList
    }.MockVM();

    Assert.IsTrue(viewmodel.SwitchCardOwnershipCommand.CanExecute(card));
  }

  [TestMethod(DisplayName = "Should not be able to execute if the given card is null")]
  public async Task InvalidParameter_CanNotExecute()
  {
    var viewmodel = await new Mocker(_dependencies).MockVM();

    Assert.IsFalse(viewmodel.SwitchCardOwnershipCommand.CanExecute(null));
  }

  [TestMethod]
  public async Task SwitchOwnership_Null_NoChanges()
  {
    var viewmodel = await new Mocker(_dependencies).MockVM();

    viewmodel.SwitchCardOwnershipCommand.Execute(null);

    Assert.IsEmpty(viewmodel.CollectionList.Cards);
  }

  [TestMethod]
  public async Task SwitchOwnership_New_CardAdded()
  {
    var card = new CardCollectionMTGCard(MTGCardInfoMocker.MockInfo());
    var viewmodel = await new Mocker(_dependencies).MockVM();

    viewmodel.SwitchCardOwnershipCommand.Execute(card);

    Assert.HasCount(1, viewmodel.CollectionList.Cards);
  }

  [TestMethod]
  public async Task SwitchOwnership_Existing_CardRemoved()
  {
    var card = new CardCollectionMTGCard(MTGCardInfoMocker.MockInfo());
    var viewmodel = await new Mocker(_dependencies)
    {
      Model = new() { Cards = [card] }
    }.MockVM();

    viewmodel.SwitchCardOwnershipCommand.Execute(card);

    Assert.IsEmpty(viewmodel.CollectionList.Cards);
  }
}
