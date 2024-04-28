using MTGApplication.General.Models.CardDeck;
using MTGApplication.General.UseCases;
using System.Threading.Tasks;

namespace MTGApplication.General.Databases.Repositories.MTGDeckRepository;

public class AddDeckUseCase : UseCase<MTGCardDeck, Task<bool>>
{
  public AddDeckUseCase(IRepository<MTGCardDeckDTO> repository) => Repository = repository;

  public IRepository<MTGCardDeckDTO> Repository { get; }

  public override async Task<bool> Execute(MTGCardDeck deck) => await Repository.Add(new(deck));
}
