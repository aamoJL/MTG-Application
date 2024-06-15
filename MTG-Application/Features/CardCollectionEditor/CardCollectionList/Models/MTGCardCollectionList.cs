using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace MTGApplication.Features.CardCollection;

/// <summary>
/// <see cref="MTGCardCollection"/>'s list class
/// </summary>
public partial class MTGCardCollectionList : ObservableObject
{
  // These are bbservable properties so combobox header will update when changed
  [ObservableProperty] private string name = string.Empty;
  [ObservableProperty] private string searchQuery = string.Empty;

  public ObservableCollection<CardCollectionMTGCard> Cards { get; set; } = [];
}