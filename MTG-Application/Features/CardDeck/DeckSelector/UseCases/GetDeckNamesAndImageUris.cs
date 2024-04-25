using MTGApplication.API.CardAPI;
using MTGApplication.General.Databases.Repositories;
using MTGApplication.General.Databases.Repositories.MTGDeckRepository;
using MTGApplication.General.Models.Card;
using MTGApplication.General.UseCases;
using MTGApplication.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MTGApplication.Features.CardDeck;
public class GetDeckNamesAndImageUris : UseCase<Task<IEnumerable<(string Name, string ImageUri)>>>
{
  public GetDeckNamesAndImageUris(IRepository<MTGCardDeckDTO> repository, ICardAPI<MTGCard> cardAPI)
  {
    Repository = repository;
    CardAPI = cardAPI;
  }

  public IRepository<MTGCardDeckDTO> Repository { get; }
  public ICardAPI<MTGCard> CardAPI { get; }

  public async override Task<IEnumerable<(string Name, string ImageUri)>> Execute()
  {
    var decks = await new GetDecksUseCase(Repository, CardAPI)
    {
      Includes = new Expression<Func<MTGCardDeckDTO, object>>[]
    {
      x => x.Commander
    }
    }
    .Execute();

    return decks.Select(x => (x.Name, x.Commander?.Info.FrontFace.ImageUri ?? string.Empty));
  }
}
