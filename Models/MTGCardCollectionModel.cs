using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MTGApplication.Models
{
  public partial class MTGCardCollectionModel : ObservableObject
  {
    public enum SortDirection
    {
      ASC, DESC
    }
    public enum SortProperty
    {
      CMC, Name, Rarity, Color, Set, Count, Price
    }

    public MTGCardCollectionModel()
    {
      Cards.CollectionChanged += Cards_CollectionChanged;
    }

    private void Cards_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
      switch (e.Action)
      {
        case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
          (e.NewItems[0] as MTGCard).PropertyChanged += MTGCardCollectionModel_PropertyChanged; 
          OnPropertyChanged(nameof(TotalCount)); break;
        case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
          (e.OldItems[0] as MTGCard).PropertyChanged -= MTGCardCollectionModel_PropertyChanged; 
          OnPropertyChanged(nameof(TotalCount)); break;
        case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
          OnPropertyChanged(nameof(TotalCount)); break;
        case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
          break;
        case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
        default:
          throw new NotImplementedException();
      }
    }
    private void MTGCardCollectionModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      if(e.PropertyName == nameof(MTGCard.Count))
        OnPropertyChanged(nameof(TotalCount));
    }

    public ObservableCollection<MTGCard> Cards { get; } = new();
    public int TotalCount { get => Cards.Sum(x => x.Count); }

    public void SortCollection(SortDirection dir, SortProperty prop)
    {
      List<MTGCard> newList = new();
      if (dir == SortDirection.ASC)
      {
        newList = prop switch
        {
          SortProperty.CMC => Cards.OrderBy(x => x.Info.CMC).ToList(),
          SortProperty.Name => Cards.OrderBy(x => x.Info.Name).ToList(),
          SortProperty.Rarity => Cards.OrderBy(x => (int)x.Info.RarityType).ToList(),
          SortProperty.Color => Cards.OrderBy(x => (int)x.Info.ColorType).ToList(),
          SortProperty.Set => Cards.OrderBy(x => x.Info.SetName).ToList(),
          SortProperty.Count => Cards.OrderBy(x => x.Count).ToList(),
          SortProperty.Price => Cards.OrderBy(x => x.Info.Price).ToList(),
          _ => throw new NotImplementedException(),
        };
      }
      else
      {
        newList = prop switch
        {
          SortProperty.CMC => Cards.OrderByDescending(x => x.Info.CMC).ToList(),
          SortProperty.Name => Cards.OrderByDescending(x => x.Info.Name).ToList(),
          SortProperty.Rarity => Cards.OrderByDescending(x => (int)x.Info.RarityType).ToList(),
          SortProperty.Color => Cards.OrderByDescending(x => (int)x.Info.ColorType).ToList(),
          SortProperty.Set => Cards.OrderByDescending(x => x.Info.SetName).ToList(),
          SortProperty.Count => Cards.OrderByDescending(x => x.Count).ToList(),
          SortProperty.Price => Cards.OrderByDescending(x => x.Info.Price).ToList(),
          _ => throw new NotImplementedException(),
        };
      }

      for (int i = 0; i < newList.Count; i++)
      {
        Cards.Move(Cards.IndexOf(newList[i]), i);
      }
    }
    public void Add(MTGCard model, bool combineName = true)
    {
      if (combineName)
      {
        if (Cards.FirstOrDefault(x => x.Info.Name == model.Info.Name) is MTGCard existingModel)
        {
          existingModel.Count += model.Count;
        }
        else { Cards.Add(model); }
      }
      else { Cards.Add(model); }
    }
    public void Remove(MTGCard model) => Cards.Remove(model);
    public void Clear() => Cards.Clear();
  }
}
