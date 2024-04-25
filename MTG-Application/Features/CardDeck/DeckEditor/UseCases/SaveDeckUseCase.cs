using MTGApplication.General.Databases.Repositories;
using MTGApplication.General.Databases.Repositories.MTGDeckRepository;
using MTGApplication.General.Services.ConfirmationService;
using MTGApplication.General.UseCases;
using MTGApplication.General.ViewModels;
using MTGApplication.Models;
using MTGApplication.Models.DTOs;
using System.Threading.Tasks;

namespace MTGApplication.Features.CardDeck;
public class SaveDeckUseCase : UseCase<SaveDeckUseCase.Args, Task<ConfirmationResult>>
{
  public record Args(MTGCardDeck Deck, string SaveName = null);

  public SaveDeckUseCase(IRepository<MTGCardDeckDTO> repository) => Repository = repository;

  public IRepository<MTGCardDeckDTO> Repository { get; }

  public Confirmation<string, string> SaveConfirmation { get; set; } = new();
  public Confirmation<ConfirmationResult> OverrideConfirmation { get; set; } = new();
  public IWorker Worker { get; set; }

  public override async Task<ConfirmationResult> Execute(Args args)
  {
    var (deck, saveName) = args;

    if (!string.IsNullOrEmpty(saveName ??= await SaveConfirmation.Confirm(title: "Save your deck?", message: string.Empty, deck.Name)))
    {
      var overrideOld = false;

      if (saveName != deck.Name && await new DeckExistsUseCase(Repository).Execute(saveName))
      {
        // Deck with the given name exists already
        var overrideResult = await OverrideConfirmation.Confirm(
          title: "Override existing deck?",
          message: $"Deck '{saveName}' already exist. Would you like to override the deck?");

        switch (overrideResult)
        {
          case ConfirmationResult.Success: overrideOld = true; break;
          case ConfirmationResult.Failure: return ConfirmationResult.Failure;
          default: return ConfirmationResult.Cancel;
        }
      }

      var saveTask = Save(deck, saveName, overrideOld, removeOld: saveName != deck.Name);
      var saveResult = Worker != null ? await Worker.DoWork(saveTask) : await saveTask;

      return saveResult ? ConfirmationResult.Success : ConfirmationResult.Failure;
    }

    return ConfirmationResult.Cancel;
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
}