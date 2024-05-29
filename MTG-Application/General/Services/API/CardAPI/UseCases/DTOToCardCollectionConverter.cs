using MTGApplication.General.Models.Card;
using MTGApplication.General.Models.CardCollection;
using MTGApplication.General.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace MTGApplication.General.Services.API.CardAPI;

public class DTOToCardCollectionConverter(ICardAPI<MTGCard> cardAPI) : UseCase<MTGCardCollectionDTO, Task<MTGCardCollection>>
{
  private ICardAPI<MTGCard> CardAPI { get; } = cardAPI;

  public override async Task<MTGCardCollection> Execute(MTGCardCollectionDTO dto)
  {
    return new MTGCardCollection()
    {
      Name = dto.Name,
      CollectionLists = new(await Task.WhenAll(dto.CollectionLists.Select(async x =>
      {
        return new MTGCardCollectionList()
        {
          Name = x.Name,
          SearchQuery = x.SearchQuery,
          Cards = new((await CardAPI.FetchFromDTOs(x.Cards.ToArray())).Found),
        };
      })))
    };
  }
}
