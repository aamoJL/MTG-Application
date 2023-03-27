using CommunityToolkit.Mvvm.ComponentModel;
using MTGApplication.Interfaces;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MTGApplication.Models;

/// <summary>
/// Collection, that has <see cref="MTGCard"/> objects in <see cref="CollectionLists"/>
/// </summary>
public partial class MTGCardCollection : ObservableObject
{
  public MTGCardCollection() { }

  [ObservableProperty]
  private string name = string.Empty;

  public ObservableCollection<MTGCardCollectionList> CollectionLists { get; set; } = new();

  /// <summary>
  /// Returns the collection as a data transfer object
  /// </summary>
  public MTGCardCollectionDTO AsDTO() => new(this);
}

/// <summary>
/// Data transfer object for <see cref="MTGCardCollection"/> class
/// </summary>
public class MTGCardCollectionDTO
{
  private MTGCardCollectionDTO() { }
  public MTGCardCollectionDTO(MTGCardCollection collection)
  {
    Name = collection.Name;
    CollectionLists = collection.CollectionLists.Select(x => new MTGCardCollectionListDTO(x)).ToList();
  }

  [Key]
  public int Id { get; init; }
  public string Name { get; init; }

  [InverseProperty(nameof(MTGCardCollectionListDTO.Collection))]
  public List<MTGCardCollectionListDTO> CollectionLists { get; init; } = new();

  /// <summary>
  /// Converts the DTO to a <see cref="MTGCardCollection"/> object using the given <paramref name="api"/>
  /// </summary>
  public async Task<MTGCardCollection> AsMTGCardCollection(ICardAPI<MTGCard> api)
  {
    return new MTGCardCollection()
    {
      Name = Name,
      CollectionLists = new(await Task.WhenAll(CollectionLists.Select(async x =>
      {
        return new MTGCardCollectionList()
        {
          Name = x.Name,
          SearchQuery = x.SearchQuery,
          Cards = new((await api.FetchFromDTOs(x.Cards.ToArray())).Found),
        };
      })))
    };
  }
}
