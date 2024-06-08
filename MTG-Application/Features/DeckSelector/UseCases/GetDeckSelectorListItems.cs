using MTGApplication.General.Databases.Repositories.DeckRepository;
using MTGApplication.General.Models.Card;
using MTGApplication.General.Services.API.CardAPI;
using MTGApplication.General.Services.Databases.Repositories;
using MTGApplication.General.Services.Databases.Repositories.DeckRepository.Models;
using MTGApplication.General.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MTGApplication.Features.DeckSelector.UseCases;
public class GetDeckSelectorListItems : UseCase<Task<IEnumerable<(string Name, string ImageUri)>>>
{
  public GetDeckSelectorListItems(IRepository<MTGCardDeckDTO> repository, ICardAPI<DeckEditorMTGCard> cardAPI)
  {
    Repository = repository;
    CardAPI = cardAPI;
  }

  public IRepository<MTGCardDeckDTO> Repository { get; }
  public ICardAPI<DeckEditorMTGCard> CardAPI { get; }

  public async override Task<IEnumerable<(string Name, string ImageUri)>> Execute()
  {
    var decks = await new GetDecks(Repository, CardAPI)
    {
      Includes = new Expression<Func<MTGCardDeckDTO, object>>[] { x => x.Commander }
    }
    .Execute();

    return decks.Select(x => (x.Name, x.Commander?.Info.FrontFace.ImageUri ?? string.Empty));
  }
}
