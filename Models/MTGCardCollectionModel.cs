using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MTGApplication.Models
{
  public class MTGCardCollectionModel
  {
    public enum SortDirection
    {
      ASC, DESC
    }
    public enum SortProperty
    {
      CMC, Name, Rarity, Color, Set
    }

    public ObservableCollection<MTGCardModel> Cards { get; } = new();

    public string Name { get; private set; }
    public int TotalCount { get => Cards.Sum(x => x.Count); }

    public void Rename(string name)
    {
      Name = name;
    }
    public void SortCollection(SortDirection dir, SortProperty prop)
    {
      List<MTGCardModel> newList = new();
      if (dir == SortDirection.ASC)
      {
        newList = prop switch
        {
          SortProperty.CMC => Cards.OrderBy(x => x.Info.CMC).ToList(),
          SortProperty.Name => Cards.OrderBy(x => x.Info.Name).ToList(),
          SortProperty.Rarity => Cards.OrderBy(x => (int)x.RarityType).ToList(),
          SortProperty.Color => Cards.OrderBy(x => (int)x.ColorType).ToList(),
          SortProperty.Set => Cards.OrderBy(x => x.Info.SetName).ToList(),
          _ => Cards.OrderBy(x => x.Info.Name).ToList(),
        };
      }
      else
      {
        newList = prop switch
        {
          SortProperty.CMC => Cards.OrderByDescending(x => x.Info.CMC).ToList(),
          SortProperty.Name => Cards.OrderByDescending(x => x.Info.Name).ToList(),
          SortProperty.Rarity => Cards.OrderByDescending(x => (int)x.RarityType).ToList(),
          SortProperty.Color => Cards.OrderByDescending(x => (int)x.ColorType).ToList(),
          SortProperty.Set => Cards.OrderByDescending(x => x.Info.SetName).ToList(),
          _ => Cards.OrderByDescending(x => x.Info.Name).ToList(),
        };
      }

      for (int i = 0; i < newList.Count; i++)
      {
        Cards.Move(Cards.IndexOf(newList[i]), i);
      }
    }
    public void Add(MTGCardModel model, bool combineName = true)
    {
      if (combineName)
      {
        var existingModelName = Cards.FirstOrDefault(x => x.Info.Name == model.Info.Name);
        if (existingModelName != null)
        {
          existingModelName.Count += model.Count;
        }
        else
        {
          Cards.Add(model);
        }
      }
      else
      {
        Cards.Add(model);
      }
    }
    public void Remove(MTGCardModel model)
    {
      Cards.Remove(model);
    }
    public void Reset()
    {
      Name = "";
      Cards.Clear();
    }
  }
}
