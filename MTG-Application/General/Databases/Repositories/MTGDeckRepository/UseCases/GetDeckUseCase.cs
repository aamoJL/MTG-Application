using MTGApplication.API.CardAPI;
using MTGApplication.General.UseCases;
using MTGApplication.Models;
using MTGApplication.Models.DTOs;
using System.Threading.Tasks;

namespace MTGApplication.General.Databases.Repositories.MTGDeckRepository;

public class GetDeckUseCase : UseCase<Task<MTGCardDeck>>
{
  public GetDeckUseCase(string name, IRepository<MTGCardDeckDTO> repository, ICardAPI<MTGCard> cardAPI)
  {
    Name = name;
    Repository = repository;
    CardAPI = cardAPI;
  }

  public string Name { get; }
  public IRepository<MTGCardDeckDTO> Repository { get; }
  public ICardAPI<MTGCard> CardAPI { get; }

  public async override Task<MTGCardDeck> Execute()
  {
    if (string.IsNullOrEmpty(Name)) return null;

    return await (await Repository.Get(Name)).AsMTGCardDeck(CardAPI);
  }
}