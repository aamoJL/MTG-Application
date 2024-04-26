using MTGApplication.General.Databases.Repositories;
using MTGApplication.General.Databases.Repositories.MTGDeckRepository;
using MTGApplication.General.UseCases;
using MTGApplication.General.ViewModels;
using MTGApplication.Models;
using MTGApplication.Models.DTOs;
using System.Threading.Tasks;
using static MTGApplication.General.Services.ConfirmationService.ConfirmationService;

namespace MTGApplication.Features.CardDeck;
public class SaveDeckUseCase : UseCase<SaveDeckUseCase.Args, Task<ConfirmationResult>>
{
  public record Args(MTGCardDeck Deck, string SaveName = null);

  public SaveDeckUseCase(IRepository<MTGCardDeckDTO> repository) => Repository = repository;

  public IRepository<MTGCardDeckDTO> Repository { get; }

  public Confirmer<string, string> SaveConfirmation { get; set; } = new();
  public Confirmer<ConfirmationResult> OverrideConfirmation { get; set; } = new();
  public IWorker Worker { get; set; } = new DefaultWorker();

  public override async Task<ConfirmationResult> Execute(Args args)
  {
    var deck = args.Deck;
    var saveName = args.SaveName ?? await SaveConfirmation.Confirm(new(Title: "Save your deck?", Message: string.Empty, deck.Name));

    if (string.IsNullOrEmpty(saveName)) return ConfirmationResult.Cancel;

    var overrideResult = await ConfirmOverride(deck, saveName);

    if (overrideResult == ConfirmationResult.Cancel) return ConfirmationResult.Cancel;

    var overrideOld = overrideResult == ConfirmationResult.Yes;

    return await Worker.DoWork(Save(deck, saveName, overrideOld, removeOld: saveName != deck.Name))
        ? ConfirmationResult.Yes : ConfirmationResult.Failure;
  }

  private async Task<bool> Save(MTGCardDeck deck, string saveName, bool overrideExisting, bool removeOld)
  {
    var oldName = deck.Name;

    if (oldName != saveName && await new DeckExistsUseCase(Repository).Execute(saveName) && !overrideExisting)
      return false; // Cancel because overriding is not enabled

    if (await new AddOrUpdateDeckUseCase(Repository).Execute((deck, saveName)) is bool wasSaved && wasSaved is true)
    {
      deck.Name = saveName;

      if (oldName != saveName && removeOld is true && !string.IsNullOrEmpty(oldName))
        await new General.Databases.Repositories.MTGDeckRepository.DeleteDeckUseCase(Repository).Execute(oldName);
    }

    return wasSaved;
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
}