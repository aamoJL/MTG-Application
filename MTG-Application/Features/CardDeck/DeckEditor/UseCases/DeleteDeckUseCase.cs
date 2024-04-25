using MTGApplication.General.Databases.Repositories;
using MTGApplication.General.Databases.Repositories.MTGDeckRepository;
using MTGApplication.General.Services.ConfirmationService;
using MTGApplication.General.UseCases;
using MTGApplication.General.ViewModels;
using MTGApplication.Models;
using MTGApplication.Models.DTOs;
using System.Threading.Tasks;

namespace MTGApplication.Features.CardDeck;

public class DeleteDeckUseCase : UseCase<MTGCardDeck, Task<ConfirmationResult>>
{
  public DeleteDeckUseCase(IRepository<MTGCardDeckDTO> repository) => Repository = repository;

  public IRepository<MTGCardDeckDTO> Repository { get; }

  public Confirmation<ConfirmationResult> DeleteConfirmation { get; set; } = new();
  public IWorker Worker { get; set; }

  public override async Task<ConfirmationResult> Execute(MTGCardDeck deck)
  {
    var deleteConfirmation = await DeleteConfirmation.Confirm("Delete deck?", $"Are you sure you want to delete '{deck.Name}'?");
    
    if (!await new DeckExistsUseCase(Repository).Execute(deck.Name)) 
      return ConfirmationResult.Failure; // Could not find the deck

    switch (deleteConfirmation)
    {
      case ConfirmationResult.Success:
        var deleteTask = new General.Databases.Repositories.MTGDeckRepository.DeleteDeckUseCase(Repository).Execute(deck);
        var deleteResult = Worker != null ? await Worker.DoWork(deleteTask) : await deleteTask;

        return deleteResult switch
        {
          true => ConfirmationResult.Success,
          false => ConfirmationResult.Failure,
        };
      case ConfirmationResult.Failure: return ConfirmationResult.Failure;
      default: return ConfirmationResult.Cancel;
    }
  }
}