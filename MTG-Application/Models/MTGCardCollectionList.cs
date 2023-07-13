using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Linq;

namespace MTGApplication.Models;

/// <summary>
/// <see cref="MTGCardCollection"/>'s list class
/// </summary>
public partial class MTGCardCollectionList : ObservableObject
{
  public MTGCardCollectionList() { }

  [ObservableProperty] private string name = string.Empty;
  [ObservableProperty] private string searchQuery = string.Empty;

  public ObservableCollection<MTGCard> Cards { get; set; } = new();

  /// <summary>
  /// Adds the given <paramref name="card"/> to the list
  /// </summary>
  public bool AddToList(MTGCard card)
  {
    if (Cards.Contains(card)) return false;
    Cards.Add(card);
    return true;
  }

  /// <summary>
  /// Removes the given <paramref name="card"/> from the list
  /// </summary>
  public bool RemoveFromList(MTGCard card)
  {
    if (Cards.FirstOrDefault(x => x.Info.ScryfallId == card.Info.ScryfallId) is MTGCard existingCard)
    {
      Cards.Remove(existingCard);
      return true;
    }
    return false;
  }
}