using MTGApplication.General.Databases.Repositories;
using MTGApplication.General.Models.Card;
using MTGApplication.General.Models.CardCollection;
using MTGApplication.General.Services.API.CardAPI;
using MTGApplication.General.ViewModels;
using System.Threading.Tasks;

namespace MTGApplication.General.Services.Databases.Repositories.CardCollectionRepository.UseCases;
public class GetCardCollection(IRepository<MTGCardCollectionDTO> repository, ICardAPI<MTGCard> cardAPI) : UseCase<string, Task<MTGCardCollection>>
{
  public IRepository<MTGCardCollectionDTO> Repository { get; } = repository;
  public ICardAPI<MTGCard> CardAPI { get; } = cardAPI;

  public async override Task<MTGCardCollection> Execute(string name)
  {
    if (string.IsNullOrEmpty(name)) return null;

    var dto = await Repository.Get(name);

    return dto != null ? await new DTOToCardCollectionConverter(CardAPI).Execute(dto) : null;
  }
}