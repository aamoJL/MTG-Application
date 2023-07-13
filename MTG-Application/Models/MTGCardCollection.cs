using CommunityToolkit.Mvvm.ComponentModel;
using MTGApplication.Models.DTOs;
using System.Collections.ObjectModel;

namespace MTGApplication.Models;

/// <summary>
/// Collection, that has <see cref="MTGCard"/> objects in <see cref="CollectionLists"/>
/// </summary>
public partial class MTGCardCollection : ObservableObject
{
  public MTGCardCollection() { }

  [ObservableProperty] private string name = string.Empty;

  public ObservableCollection<MTGCardCollectionList> CollectionLists { get; set; } = new();

  /// <summary>
  /// Returns the collection as a data transfer object
  /// </summary>
  public MTGCardCollectionDTO AsDTO() => new(this);
}