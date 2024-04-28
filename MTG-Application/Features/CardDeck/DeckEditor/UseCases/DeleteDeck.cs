using MTGApplication.General.Databases.Repositories;
using MTGApplication.General.Databases.Repositories.MTGDeckRepository;
using MTGApplication.General.Models.CardDeck;
using MTGApplication.General.UseCases;
using MTGApplication.General.ViewModels;
using System.Threading.Tasks;
using static MTGApplication.General.Services.ConfirmationService.ConfirmationService;

namespace MTGApplication.Features.CardDeck;

public class DeleteDeck : UseCase<MTGCardDeck, Task<ConfirmationResult>>
{
  public DeleteDeck(IRepository<MTGCardDeckDTO> repository) => Repository = repository;

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
      ConfirmationResult.Yes => await Worker.DoWork(new DeleteDeckUseCase(Repository).Execute(deck)) switch
      {
        true => ConfirmationResult.Yes,
        false => ConfirmationResult.Failure,
      },
      _ => ConfirmationResult.Cancel,
    };
  }
}