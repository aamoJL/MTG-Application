using MTGApplication.General.Models.CardDeck;
using MTGApplication.General.UseCases;
using System.Threading.Tasks;

namespace MTGApplication.General.Databases.Repositories.DeckRepository;

public class DeckExists : UseCase<string, Task<bool>>
{
  public DeckExists(IRepository<MTGCardDeckDTO> repository) => Repository = repository;

  public IRepository<MTGCardDeckDTO> Repository { get; init; }

  public async override Task<bool> Execute(string name) => await Repository.Exists(name);
}
