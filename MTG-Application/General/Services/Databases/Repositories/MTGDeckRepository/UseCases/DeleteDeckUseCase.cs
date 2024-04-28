using MTGApplication.General.Extensions;
using MTGApplication.General.Models.CardDeck;
using MTGApplication.General.UseCases;
using System.Threading.Tasks;

namespace MTGApplication.General.Databases.Repositories.MTGDeckRepository;
public class DeleteDeckUseCase : UseCase<MTGCardDeck, Task<bool>>
{
  public DeleteDeckUseCase(IRepository<MTGCardDeckDTO> repository) => Repository = repository;

  public IRepository<MTGCardDeckDTO> Repository { get; }

  public override async Task<bool> Execute(MTGCardDeck deck) => await Repository.Delete(new(deck));

  public async Task<bool> Execute(string deckName)
    => await Repository.Delete(await Repository.Get(deckName, ExpressionExtensions.EmptyArray<MTGCardDeckDTO>()));
}