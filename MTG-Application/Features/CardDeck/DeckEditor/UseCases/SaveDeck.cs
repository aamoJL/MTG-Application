using MTGApplication.General.Databases.Repositories;
using MTGApplication.General.Databases.Repositories.MTGDeckRepository;
using MTGApplication.General.Models.CardDeck;
using MTGApplication.General.UseCases;
using MTGApplication.General.ViewModels;
using System.Threading.Tasks;
using static MTGApplication.General.Services.ConfirmationService.ConfirmationService;

namespace MTGApplication.Features.CardDeck;
public class SaveDeck : UseCase<MTGCardDeck, Task<ConfirmationResult>>
{
  public SaveDeck(IRepository<MTGCardDeckDTO> repository) => Repository = repository;

  public IRepository<MTGCardDeckDTO> Repository { get; }

  public Confirmer<string, string> SaveConfirmation { get; set; } = new();
  public Confirmer<ConfirmationResult> OverrideConfirmation { get; set; } = new();
  public IWorker Worker { get; set; } = new DefaultWorker();

  public override async Task<ConfirmationResult> Execute(MTGCardDeck deck)
  {
    var saveName = await SaveConfirmation.Confirm(new(Title: "Save your deck?", Message: string.Empty, deck.Name));

    return await Execute(deck, saveName);
  }

  public async Task<ConfirmationResult> Execute(MTGCardDeck deck, string saveName)
  {
    if (string.IsNullOrEmpty(saveName)) return ConfirmationResult.Cancel;

    var overrideResult = await ConfirmOverride(deck, saveName);

    if (overrideResult == ConfirmationResult.Cancel) return ConfirmationResult.Cancel;

    var overrideOld = overrideResult == ConfirmationResult.Yes;

    return await Worker.DoWork(Save(deck, saveName, overrideOld, removeOld: saveName != deck.Name))
        ? ConfirmationResult.Yes : ConfirmationResult.Failure;
  }

  private async Task<ConfirmationResult> ConfirmOverride(MTGCardDeck deck, string saveName)
  {
    if (saveName != deck.Name && await new DeckExistsUseCase(Repository).Execute(saveName))
    {
      // Deck with the given name exists already
      var overrideResult = await OverrideConfirmation.Confirm(new(
        Title: "Override existing deck?",
        Message: $"Deck '{saveName}' already exist. Would you like to override the deck?"));

      return overrideResult;
    }
    else return ConfirmationResult.No;
  }

  private async Task<bool> Save(MTGCardDeck deck, string saveName, bool overrideExisting, bool removeOld)
  {
    var oldName = deck.Name;

    if (oldName != saveName && await new DeckExistsUseCase(Repository).Execute(saveName) && !overrideExisting)
      return false; // Cancel because overriding is not enabled

    if (await new AddOrUpdateDeckUseCase(Repository).Execute((deck, saveName)) is bool wasSaved && wasSaved is true)
    {
      deck.Name = saveName;

      if (oldName != saveName && removeOld && await new DeckExistsUseCase(Repository).Execute(oldName) && !string.IsNullOrEmpty(oldName))
        await new DeleteDeckUseCase(Repository).Execute(oldName);
    }

    return wasSaved;
  }
}