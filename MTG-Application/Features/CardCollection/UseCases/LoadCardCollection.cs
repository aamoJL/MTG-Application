using MTGApplication.General.Databases.Repositories;
using MTGApplication.General.Models.Card;
using MTGApplication.General.Models.CardCollection;
using MTGApplication.General.Services.API.CardAPI;
using MTGApplication.General.Services.Databases.Repositories.CardCollectionRepository.UseCases;
using MTGApplication.General.ViewModels;
using System.Threading.Tasks;

namespace MTGApplication.Features.CardCollection;

public class LoadCardCollection(IRepository<MTGCardCollectionDTO> repository, ICardAPI<MTGCard> cardAPI) : UseCase<string, Task<MTGCardCollection>>
{
  public override async Task<MTGCardCollection> Execute(string loadName)
  {
    return !string.IsNullOrEmpty(loadName) switch
    {
      true => await new GetCardCollection(repository, cardAPI).Execute(loadName),
      _ => null,
    };
  }
}