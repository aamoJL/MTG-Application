using System.ComponentModel;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Collections.Specialized;
using MTGApplication.Charts;
using MTGApplication.Models;
using CommunityToolkit.Mvvm.Input;
using System;
using static MTGApplication.Models.MTGCardCollectionModel;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace MTGApplication.ViewModels
{
  public partial class MTGCardCollectionViewModel : ViewModelBase
  {
    public enum DisplayTypes
    {
      List, Images
    }

    public MTGCardCollectionViewModel(MTGCardCollectionModel model)
    {
      Model = model;
      model.Cards.CollectionChanged += Model_CollectionChanged;

      Charts = new MTGCardModelChart[]{
        new CMCChart(model.Cards),
        new SpellTypeChart(model.Cards)
      };

      _ = LoadAsync("Ostoslista");
    }

    private MTGCardCollectionModel Model { get; }
    public ObservableCollection<MTGCardViewModel> CardViewModels { get; } = new(); // Synced to Model.Cards
    public MTGCardModelChart[] Charts { get; }

    private int totalCount;
    private bool hasFile;
    private bool isLoading;
    private DisplayTypes displayType = DisplayTypes.Images;

    public int TotalCount
    {
      get => totalCount;
      set
      {
        totalCount = value;
        OnPropertyChanged(nameof(TotalCount));
        OnPropertyChanged(nameof(CanSave));
      }
    }
    public SortProperty CollectionSortProperty { get; private set; } = SortProperty.Name;
    public SortDirection CollectionSortDirection { get; private set; } = SortDirection.ASC;
    public bool HasUnsavedChanges { get; private set; }
    public bool HasFile
    {
      get => hasFile;
      set
      {
        hasFile = value;
        OnPropertyChanged(nameof(HasFile));
      }
    }
    public bool IsLoading
    {
      get => isLoading;
      set
      {
        isLoading = value;
        OnPropertyChanged(nameof(IsLoading));
      }
    }
    public string Name => Model.Name;
    public ObservableCollection<MTGCardModel> CardModels => Model.Cards;
    public bool CanSave
    {
      get => Model.TotalCount > 0;
    }
    public DisplayTypes DisplayType
    {
      get => displayType;
      set
      {
        displayType = value;
        OnPropertyChanged(nameof(DisplayType));
      }
    }

    [RelayCommand]
    public void AddAndSort(MTGCardModel model)
    {
      // TODO: insert instead of adding and sorting the whole collection
      Model.Add(model);
      Model.SortCollection(CollectionSortDirection, CollectionSortProperty);
      HasUnsavedChanges = true;
    }
    [RelayCommand]
    public void RemoveModel(MTGCardModel model)
    {
      Model.Remove(model);
      HasUnsavedChanges = true;
    }
    [RelayCommand]
    public void RemoveViewModel(MTGCardViewModel viewModel)
    {
      if (viewModel?.CardInfo.Id == null) { return; }
      Model.Remove(Model.Cards.First(x => x.Info.Id == viewModel.CardInfo.Id));
      HasUnsavedChanges = true;
    }
    [RelayCommand]
    public void Reset()
    {
      Model.Reset();
      HasUnsavedChanges = false;
      HasFile = false;
    }
    [RelayCommand]
    public void DeleteDeckFile()
    {
      if (Model.Name != "")
        IO.DeleteFile($"{IO.CollectionsPath}{Model.Name}.json");
      if (ResetCommand.CanExecute(null)) ResetCommand.Execute(null);
    }
    [RelayCommand]
    public void ChangeSortDirection(string dir)
    {
      if (Enum.TryParse(dir, true, out SortDirection sortDirection))
      {
        CollectionSortDirection = sortDirection;
        Model.SortCollection(CollectionSortDirection, CollectionSortProperty);
      }
    }
    [RelayCommand]
    public void ChangeSortProperty(string prop)
    {
      if (Enum.TryParse(prop, true, out SortProperty sortProperty))
      {
        CollectionSortProperty = sortProperty;
        Model.SortCollection(CollectionSortDirection, CollectionSortProperty);
      }
    }
    [RelayCommand]
    public void ChangeDisplayType(string type)
    {
      if (Enum.TryParse(type, true, out DisplayTypes dt))
      {
        DisplayType = dt;
      }
    }
    [RelayCommand]
    public void Save(string name)
    {
      Model.Rename(name);
      IO.WriteTextToFile($"{IO.CollectionsPath}/{name}.json",
        JsonSerializer.Serialize(Model.Cards.Select(x => new
        {
          x.Info.Id,
          x.Count
        })));
      HasUnsavedChanges = false;
      HasFile = true;
    }
    [RelayCommand]
    public async Task LoadAsync(string name)
    {
      IsLoading = true;
      Reset();
      Model.Rename(name);

      try
      {
        var ids = JsonNode.Parse(IO.ReadTextFromFile(
        $"{IO.CollectionsPath}/{name}.json")).AsArray();

        var IdObjects = ids.Select(x => new
        {
          Id = x["Id"].GetValue<string>(),
          Count = x["Count"].GetValue<int>()
        });

        // Scryfall API allows to fetch only 75 cards at a time
        foreach (var chunk in IdObjects.Chunk(75))
        {
          // Fetch cards in chunks
          var identifiersJson = JsonSerializer.Serialize(new
          {
            identifiers = chunk.Select(x => new
            {
              id = x.Id
            })
          });

          var cardCollection = await App.CardAPI.FetchCollection(identifiersJson);
          foreach (var item in cardCollection)
          {
            // Update counts to the fetched cards
            item.Count = chunk.First(x => x.Id == item.Info.Id).Count;

            Model.Add(item);
          }
        }
        Model.SortCollection(CollectionSortDirection, CollectionSortProperty);
        HasUnsavedChanges = false;
        HasFile = true;
      }
      catch (Exception) { }

      IsLoading = false;
    }
    /// <summary>
    /// Fetches cards from API and changes cards to the fetched cards
    /// </summary>
    /// <param name="query">API query parameters</param>
    /// <returns></returns>
    public async Task LoadFromAPIAsync(string query)
    {
      IsLoading = true;
      Reset();
      if (query != "")
      {
        var cards = await App.CardAPI.FetchCards(query);
        foreach (MTGCardModel card in cards)
        {
          Model.Add(card);
          // TODO: dont sort, add range
        }
      }
      HasUnsavedChanges = false;
      IsLoading = false;
    }

    private void UpdateTotalCount() => TotalCount = Model.TotalCount;
    private void Model_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      // Sync Model.Cards and CardViewModels
      MTGCardModel modelCard;

      switch (e.Action)
      {
        case NotifyCollectionChangedAction.Add:
          modelCard = e.NewItems[0] as MTGCardModel;
          var newViewModelCard = new MTGCardViewModel(modelCard) { RemoveRequestCommand = RemoveViewModelCommand };
          CardViewModels.Add(newViewModelCard);
          modelCard.PropertyChanged += ModelCard_PropertyChanged;
          break;
        case NotifyCollectionChangedAction.Remove:
          modelCard = e.OldItems[0] as MTGCardModel;
          CardViewModels.RemoveAt(e.OldStartingIndex);
          modelCard.PropertyChanged -= ModelCard_PropertyChanged;
          break;
        case NotifyCollectionChangedAction.Reset:
          foreach (var card in Model.Cards)
          {
            card.PropertyChanged -= ModelCard_PropertyChanged;
          }
          CardViewModels.Clear();
          break;
        case NotifyCollectionChangedAction.Move:
          CardViewModels.Move(e.OldStartingIndex, e.NewStartingIndex);
          break;
        case NotifyCollectionChangedAction.Replace:
        default:
          break;
      }
      UpdateTotalCount();
    }
    private void ModelCard_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == nameof(MTGCardModel.Count))
      {
        UpdateTotalCount();
        HasUnsavedChanges = true;
      }
    }
  }
}
