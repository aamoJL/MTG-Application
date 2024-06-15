using System.Collections.ObjectModel;

namespace MTGApplication.Features.CardCollection;

public class MTGCardCollection
{
  public string Name { get; set; } = string.Empty;

  public ObservableCollection<MTGCardCollectionList> CollectionLists { get; set; } = [];
}
