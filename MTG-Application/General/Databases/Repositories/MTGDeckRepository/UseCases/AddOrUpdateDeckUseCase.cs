using MTGApplication.General.UseCases;
using MTGApplication.Models.DTOs;
using System.Threading.Tasks;

namespace MTGApplication.General.Databases.Repositories.MTGDeckRepository;

public class AddOrUpdateDeckUseCase : UseCase<Task<bool>>
{
  public AddOrUpdateDeckUseCase(MTGCardDeckDTO deck, IRepository<MTGCardDeckDTO> repository)
  {
    Deck = deck;
    Repository = repository;
  }

  public MTGCardDeckDTO Deck { get; }
  public IRepository<MTGCardDeckDTO> Repository { get; }

  public override async Task<bool> Execute() => await Repository.AddOrUpdate(Deck);
}
