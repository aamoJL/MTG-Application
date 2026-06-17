using MTGApplication.General.Services.Databases.Repositories.CardCollectionRepository.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MTGApplication.Features.CardCollectionEditor.Models.Converters;

public class DTOToCardCollectionConverter
{
  /// <exception cref="ArgumentNullException"></exception>
  public async Task<MTGCardCollection> Convert(MTGCardCollectionDTO dto)
  {
    ArgumentNullException.ThrowIfNull(dto);

    return new MTGCardCollection()
    {
      Name = dto.Name,
      CollectionLists = [.. dto.CollectionLists.Select(x => new DTOToCardCollectionListConverter().Convert(x))]
    };
  }
}
