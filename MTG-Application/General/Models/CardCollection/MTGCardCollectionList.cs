using CommunityToolkit.Mvvm.ComponentModel;
using MTGApplication.General.Models.Card;
using System.Collections.ObjectModel;

namespace MTGApplication.General.Models.CardCollection;

/// <summary>
/// <see cref="MTGCardCollection"/>'s list class
/// </summary>
public partial class MTGCardCollectionList : ObservableObject
{
  [ObservableProperty] private string name = string.Empty;
  [ObservableProperty] private string searchQuery = string.Empty;

  public ObservableCollection<MTGCard> Cards { get; set; } = [];
}