using MTGApplication.General.Models.Card;
using MTGApplication.General.Services.API.CardAPI;
using MTGApplication.General.Services.Databases.Repositories;
using MTGApplication.General.Services.Databases.Repositories.CardCollectionRepository.Models;
using MTGApplication.General.Services.Databases.Repositories.CardCollectionRepository.UseCases;
using MTGApplication.General.ViewModels;
using System.Threading.Tasks;

namespace MTGApplication.Features.CardCollection.UseCases;

public class LoadCardCollection(IRepository<MTGCardCollectionDTO> repository, ICardAPI<DeckEditorMTGCard> cardAPI) : UseCase<string, Task<MTGCardCollection>>
{
  public override async Task<MTGCardCollection> Execute(string loadName)
  {
    return !string.IsNullOrEmpty(loadName) switch
    {
      true => await new GetCardCollectionDTO(repository, cardAPI).Execute(loadName),
      _ => null,
    };
  }
}