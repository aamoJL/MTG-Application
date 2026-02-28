using System.Collections.ObjectModel;
using System.ComponentModel;

namespace MTGApplication.Features.CardCollectionEditor.Models;

public partial class MTGCardCollection : INotifyPropertyChanged, INotifyPropertyChanging
{
  public string Name
  {
    get;
    set
    {
      if (field != value)
      {
        PropertyChanging?.Invoke(this, new(nameof(Name)));
        field = value;
        PropertyChanged?.Invoke(this, new(nameof(Name)));
      }
    }
  } = string.Empty;

  public ObservableCollection<MTGCardCollectionList> CollectionLists { get; set; } = [];

  public event PropertyChangedEventHandler? PropertyChanged;
  public event PropertyChangingEventHandler? PropertyChanging;
}
