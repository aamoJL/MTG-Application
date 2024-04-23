using MTGApplication.API.CardAPI;
using MTGApplication.Database;
using MTGApplication.General.Databases.Repositories.MTGDeckRepository;
using MTGApplication.General.UseCases;
using MTGApplication.Models;
using MTGApplication.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MTGApplication.Features.CardDeck;
public class GetMTGDeckNamesAndImageUris : UseCase<Task<IEnumerable<(string Name, string ImageUri)>>>
{
  public GetMTGDeckNamesAndImageUris(CardDbContextFactory contextFactory, ICardAPI<MTGCard> cardAPI)
  {
    ContextFactory = contextFactory;
    CardAPI = cardAPI;
  }

  public CardDbContextFactory ContextFactory { get; }
  public ICardAPI<MTGCard> CardAPI { get; }

  public async override Task<IEnumerable<(string Name, string ImageUri)>> Execute()
  {
    var decks = await new GetDecksUseCase(new DeckDTORepository(), CardAPI)
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
