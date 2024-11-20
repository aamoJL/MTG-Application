using CommunityToolkit.Mvvm.ComponentModel;
using MTGApplication.Features.CardCollectionEditor.CardCollection.Models;
using System.Collections.ObjectModel;

namespace MTGApplication.Features.CardCollectionEditor.CardCollectionList.Models;

/// <summary>
/// <see cref="MTGCardCollection"/>'s list class
/// </summary>
public partial class MTGCardCollectionList : ObservableObject
{
  // These are bbservable properties so combobox header will update when changed
  [ObservableProperty] public partial string Name { get; set; } = string.Empty;
  [ObservableProperty] public partial string SearchQuery { get; set; } = string.Empty;

  public ObservableCollection<CardCollectionMTGCard> Cards { get; set; } = [];
}