using MTGApplication.Features.CardCollectionEditor.CardCollectionList.Models;
using System.Collections.ObjectModel;

namespace MTGApplication.Features.CardCollectionEditor.CardCollection.Models;

public class MTGCardCollection
{
  public string Name { get; set; } = string.Empty;

  public ObservableCollection<MTGCardCollectionList> CollectionLists { get; set; } = [];
}
