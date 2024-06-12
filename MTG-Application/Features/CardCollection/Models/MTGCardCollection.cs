using CommunityToolkit.Mvvm.ComponentModel;
using MTGApplication.Features.DeckEditor.Editor.Models;
using System.Collections.ObjectModel;

namespace MTGApplication.Features.CardCollection;

/// <summary>
/// Collection, that has <see cref="DeckEditorMTGCard"/> objects in <see cref="CollectionLists"/>
/// </summary>
public partial class MTGCardCollection : ObservableObject
{
  [ObservableProperty] private string name = string.Empty;

  public ObservableCollection<MTGCardCollectionList> CollectionLists { get; set; } = [];
}
