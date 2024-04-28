using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Features.CardDeck;
using MTGApplication.General.Models.CardDeck;
using MTGApplicationTests.TestUtility;
using static MTGApplication.General.Services.ConfirmationService.ConfirmationService;

namespace MTGApplicationTests.FeatureTests.CardDeckTests.DeckEditorTests;

[TestClass]
public class SaveDeckTests
{
  private readonly UseCaseDependencies _dependencies = new();
  private readonly MTGCardDeck _deck = new() { Name = "Deck" };

  [TestMethod("Save confirmation should be shown when executed without a name")]
  [ExpectedException(typeof(ConfirmationException))]
  public async Task Execute_SaveConfirmShown()
  {
    await new SaveDeck(_dependencies.Repository)
    {
      SaveConfirmation = new TestExceptionConfirmer<string, string>()
    }.Execute(new(Deck: _deck));
  }

  [TestMethod("Save confirmation should not be shown when executed without a name")]
  public async Task Execute_WithName_SaveConfirmNotShown()
  {
    await new SaveDeck(_dependencies.Repository)
    {
      SaveConfirmation = new TestExceptionConfirmer<string, string>()
    }.Execute(new(Deck: _deck, SaveName: "new name"));
  }

  [TestMethod("Should return YES when saving with the same name")]
  public async Task Execute_Yes_ReturnYes()
  {
    var result = await new SaveDeck(_dependencies.Repository)
    {
      SaveConfirmation = new Confirmer<string, string>()
      {
        OnConfirm = async (arg) => { return await Task.FromResult(_deck.Name); }
      }
    }.Execute(new(Deck: _deck));

    Assert.AreEqual(ConfirmationResult.Yes, result);
  }

  [TestMethod("Override confirmation should not be shown if saving with the same name")]
  public async Task Execute_SameName_NoOverrideShown()
  {
    var result = await new SaveDeck(_dependencies.Repository)
    {
      SaveConfirmation = new Confirmer<string, string>()
      {
        OnConfirm = async (arg) => { return await Task.FromResult(_deck.Name); }
      },
      OverrideConfirmation = new TestExceptionConfirmer<ConfirmationResult>(),
    }.Execute(new(Deck: _deck));
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
    }.Execute(new(Deck: _deck));

    Assert.AreEqual(ConfirmationResult.Cancel, result);
  }

  [TestMethod("Override confirmation should be shown when trying to override a deck")]
  [ExpectedException(typeof(ConfirmationException))]
  public async Task Execute_Overriding_OverrideConfirmationShown()
  {
    var overrideName = "Deck 2";
    _dependencies.ContextFactory.Populate(new MTGCardDeckDTO(overrideName));

    await new SaveDeck(_dependencies.Repository)
    {
      SaveConfirmation = new Confirmer<string, string>()
      {
        OnConfirm = async (arg) => { return await Task.FromResult(overrideName); }
      },
      OverrideConfirmation = new TestExceptionConfirmer<ConfirmationResult>()
    }.Execute(new(Deck: _deck));
  }

  [TestMethod("Should return YES when accepting overriding")]
  [ExpectedException(typeof(ConfirmationException))]
  public async Task Execute_AcceptOverriding_ReturnYes()
  {
    var overrideName = "Deck 2";
    _dependencies.ContextFactory.Populate(new MTGCardDeckDTO(overrideName));

    var result = await new SaveDeck(_dependencies.Repository)
    {
      SaveConfirmation = new Confirmer<string, string>()
      {
        OnConfirm = async (arg) => { return await Task.FromResult(overrideName); }
      },
      OverrideConfirmation = new TestExceptionConfirmer<ConfirmationResult>()
    }.Execute(new(Deck: _deck));

    Assert.AreEqual(ConfirmationResult.Yes, result);
  }

  [TestMethod("Should return CANCEL when canceling overriding")]
  public async Task Execute_CancelOverriding_ReturnCancel()
  {
    var overrideName = "Deck 2";
    _dependencies.ContextFactory.Populate(new MTGCardDeckDTO(overrideName));

    var result = await new SaveDeck(_dependencies.Repository)
    {
      SaveConfirmation = new Confirmer<string, string>()
      {
        OnConfirm = async (arg) => { return await Task.FromResult(overrideName); }
      },
      OverrideConfirmation = new Confirmer<ConfirmationResult>()
      {
        OnConfirm = async (arg) => { return await Task.FromResult(ConfirmationResult.Cancel); }
      },
    }.Execute(new(Deck: _deck));

    Assert.AreEqual(ConfirmationResult.Cancel, result);
  }
}