using MTGApplication.General.Databases.Repositories;
using MTGApplication.General.Databases.Repositories.MTGDeckRepository;
using MTGApplication.General.UseCases;
using MTGApplication.General.ViewModels;
using MTGApplication.Models;
using MTGApplication.Models.DTOs;
using System.Threading.Tasks;
using static MTGApplication.General.Services.ConfirmationService.ConfirmationService;

namespace MTGApplication.Features.CardDeck;

public class DeleteDeckUseCase : UseCase<MTGCardDeck, Task<ConfirmationResult>>
{
  public DeleteDeckUseCase(IRepository<MTGCardDeckDTO> repository) => Repository = repository;

  public IRepository<MTGCardDeckDTO> Repository { get; }

  public Confirmer<ConfirmationResult> DeleteConfirmation { get; set; } = new();
  public IWorker Worker { get; set; } = new DefaultWorker();

  public override async Task<ConfirmationResult> Execute(MTGCardDeck deck)
  {
    var deleteConfirmation = await DeleteConfirmation.Confirm(new("Delete deck?", $"Are you sure you want to delete '{deck.Name}'?"));

    if (!await new DeckExistsUseCase(Repository).Execute(deck.Name))
      return ConfirmationResult.Failure; // Could not find the deck

    return deleteConfirmation switch
    {
      ConfirmationResult.Yes => await Worker.DoWork(new General.Databases.Repositories.MTGDeckRepository.DeleteDeckUseCase(Repository).Execute(deck)) switch
      {
        true => ConfirmationResult.Yes,
        false => ConfirmationResult.Failure,
      },
      _ => ConfirmationResult.Cancel,
    };
  }
}