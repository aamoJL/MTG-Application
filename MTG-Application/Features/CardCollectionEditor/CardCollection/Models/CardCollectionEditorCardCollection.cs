using MTGApplication.Features.CardCollectionEditor.CardCollectionList.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MTGApplication.Features.CardCollectionEditor.CardCollection.Models;

public partial class CardCollectionEditorCardCollection : INotifyPropertyChanged, INotifyPropertyChanging
{
  public string Name
  {
    get;
    set => SetProperty(ref field, value);
  } = string.Empty;

  public ObservableCollection<MTGCardCollectionList> CollectionLists { get; set; } = [];

  public event PropertyChangedEventHandler? PropertyChanged;
  public event PropertyChangingEventHandler? PropertyChanging;

  private void SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
  {
    if (!EqualityComparer<T>.Default.Equals(field, value))
    {
      PropertyChanging?.Invoke(this, new(propertyName));
      field = value;
      PropertyChanged?.Invoke(this, new(propertyName));
    }
  }
}
