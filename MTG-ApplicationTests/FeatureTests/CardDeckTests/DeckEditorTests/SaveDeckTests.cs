using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Features.CardDeck;
using MTGApplication.General.Models.CardDeck;
using MTGApplicationTests.TestUtility;
using static MTGApplication.General.Services.ConfirmationService.ConfirmationService;

namespace MTGApplicationTests.FeatureTests.CardDeckTests.DeckEditorTests;

[TestClass]
public class SaveDeckTests
{
  private readonly RepositoryDependencies _dependencies = new();
  private readonly MTGCardDeck _savedDeck = new() { Name = "SavedDeck" };

  public SaveDeckTests() => _dependencies.ContextFactory.Populate(new MTGCardDeckDTO(_savedDeck));

  [TestMethod("Save confirmation should be shown when executed without a name")]
  [ExpectedException(typeof(ConfirmationException))]
  public async Task Execute_SaveConfirmShown()
  {
    await new SaveDeck(_dependencies.Repository)
    {
      SaveConfirmation = new TestExceptionConfirmer<string, string>()
    }.Execute(_savedDeck);
  }

  [TestMethod("Save confirmation should not be shown when executed with a name")]
  public async Task Execute_WithName_SaveConfirmNotShown()
  {
    await new SaveDeck(_dependencies.Repository)
    {
      SaveConfirmation = new TestExceptionConfirmer<string, string>()
    }.Execute(_savedDeck, "New name");
  }

  [TestMethod("Override confirmation should be shown when executed with an existing name")]
  [ExpectedException(typeof(ConfirmationException))]
  public async Task Execute_WithExistingName_OverrideConfirmShown()
  {
    var newDeck = new MTGCardDeck() { Name = "New Deck" };

    await new SaveDeck(_dependencies.Repository)
    {
      OverrideConfirmation = new TestExceptionConfirmer<ConfirmationResult>()
    }.Execute(newDeck, _savedDeck.Name);
  }

  [TestMethod("Should return YES when saving with the same name")]
  public async Task Execute_Yes_ReturnYes()
  {
    var result = await new SaveDeck(_dependencies.Repository)
    {
      SaveConfirmation = new Confirmer<string, string>()
      {
        OnConfirm = async (arg) => { return await Task.FromResult(_savedDeck.Name); }
      }
    }.Execute(_savedDeck);

    Assert.AreEqual(ConfirmationResult.Yes, result);
  }

  [TestMethod("Override confirmation should not be shown if saving with the same name")]
  public async Task Execute_SameName_NoOverrideShown()
  {
    var result = await new SaveDeck(_dependencies.Repository)
    {
      SaveConfirmation = new Confirmer<string, string>()
      {
        OnConfirm = async (arg) => { return await Task.FromResult(_savedDeck.Name); }
      },
      OverrideConfirmation = new TestExceptionConfirmer<ConfirmationResult>(),
    }.Execute(_savedDeck);
  }

  [TestMethod("Should return CANCEL when canceling the saving")]
  public async Task Execute_Cancel_ReturnCancel()
  {
    var result = await new SaveDeck(_dependencies.Repository)
    {
      SaveConfirmation = new Confirmer<string, string>()
      {
        OnConfirm = async (arg) => { return await Task.FromResult(string.Empty); }
      }
    }.Execute(_savedDeck);

    Assert.AreEqual(ConfirmationResult.Cancel, result);
  }

  [TestMethod("Should return YES when accepting overriding")]
  public async Task Execute_AcceptOverriding_ReturnYes()
  {
    var newDeck = new MTGCardDeck() { Name = "New Deck" };

    var result = await new SaveDeck(_dependencies.Repository)
    {
      SaveConfirmation = new Confirmer<string, string>()
      {
        OnConfirm = (arg) => Task.FromResult(_savedDeck.Name)
      },
      OverrideConfirmation = new Confirmer<ConfirmationResult>()
      {
        OnConfirm = (arg) => Task.FromResult(ConfirmationResult.Yes)
      }
    }.Execute(newDeck);

    Assert.AreEqual(ConfirmationResult.Yes, result);
  }

  [TestMethod("Should return CANCEL when canceling overriding")]
  public async Task Execute_CancelOverriding_ReturnCancel()
  {
    var newDeck = new MTGCardDeck() { Name = "New Deck" };

    var result = await new SaveDeck(_dependencies.Repository)
    {
      SaveConfirmation = new Confirmer<string, string>()
      {
        OnConfirm = async (arg) => { return await Task.FromResult(_savedDeck.Name); }
      },
      OverrideConfirmation = new Confirmer<ConfirmationResult>()
      {
        OnConfirm = async (arg) => { return await Task.FromResult(ConfirmationResult.Cancel); }
      },
    }.Execute(newDeck);

    Assert.AreEqual(ConfirmationResult.Cancel, result);
  }

  [TestMethod("Should return FAILURE when the deck could not be saved")]
  public async Task Execute_Failure_ReturnFailure()
  {
    _dependencies.Repository.UpdateFailure = true;

    var result = await new SaveDeck(_dependencies.Repository)
    {
      SaveConfirmation = new Confirmer<string, string>()
      {
        OnConfirm = async (arg) => { return await Task.FromResult(_savedDeck.Name); }
      },
    }.Execute(_savedDeck);

    Assert.AreEqual(ConfirmationResult.Failure, result);
  }
}