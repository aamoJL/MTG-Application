using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace MTGApplication.Models
{
  public partial class MTGCardCollectionList : ObservableObject
  {
    public MTGCardCollectionList() { }

    [ObservableProperty]
    private string name = string.Empty;
    [ObservableProperty]
    private string searchQuery = string.Empty;

    public ObservableCollection<MTGCard> Cards { get; set; } = new();
  }

  public class MTGCardCollectionListDTO
  {
    private MTGCardCollectionListDTO() { }
    public MTGCardCollectionListDTO(MTGCardCollectionList list)
    {
      Name = list.Name;
      SearchQuery = list.SearchQuery;
      Cards.AddRange(list.Cards.Select(x => new MTGCardDTO(x)));
    }

    [Key]
    public int Id { get; init; }
    public string Name { get; set; }
    public string SearchQuery { get; set; }
    public MTGCardCollectionDTO Collection { get; set; }

    [InverseProperty(nameof(MTGCardDTO.CollectionList))]
    public List<MTGCardDTO> Cards { get; init; } = new();
  }
}
