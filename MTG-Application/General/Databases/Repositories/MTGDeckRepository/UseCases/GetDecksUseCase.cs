using MTGApplication.API.CardAPI;
using MTGApplication.Models;
using MTGApplication.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MTGApplication.General.Databases.Repositories.MTGDeckRepository;
public class GetDecksUseCase : UseCase<Task<IEnumerable<MTGCardDeck>>>
{
  public GetDecksUseCase(IRepository<MTGCardDeckDTO> repository, ICardAPI<MTGCard> cardAPI)
  {
    Repository = repository;
    CardAPI = cardAPI;
  }

  public IRepository<MTGCardDeckDTO> Repository { get; init; }
  public ICardAPI<MTGCard> CardAPI { get; init; }
  public Expression<Func<MTGCardDeckDTO, object>>[] Includes { get; init; } = MTGCardDeckDTO.DefaultIncludes;

  public async override Task<IEnumerable<MTGCardDeck>> Execute()
  {
    return await Task.WhenAll(
      tasks: (await Repository.Get(Includes)).Select(x => x.AsMTGCardDeck(CardAPI)));
  }
}