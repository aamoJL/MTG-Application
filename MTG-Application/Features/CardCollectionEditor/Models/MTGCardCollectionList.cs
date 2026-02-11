using CommunityToolkit.Mvvm.ComponentModel;
using MTGApplication.General.Models;
using System.Collections.ObjectModel;

namespace MTGApplication.Features.CardCollectionEditor.Models;

/// <summary>
/// <see cref="MTGCardCollection"/>'s list class
/// </summary>
public partial class MTGCardCollectionList : ObservableObject
{
  [ObservableProperty] public partial string Name { get; set; } = string.Empty;
  [ObservableProperty] public partial string SearchQuery { get; set; } = string.Empty;

  public ObservableCollection<MTGCard> Cards { get; set; } = [];
}