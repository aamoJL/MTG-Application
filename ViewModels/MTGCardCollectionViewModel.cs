using System.ComponentModel;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using MTG_builder;
using System.Collections.Specialized;
using MTGApplication.Charts;
using MTGApplication.Models;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using System;
using static MTGApplication.Models.MTGCardCollectionModel;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using App1.API;

namespace MTGApplication.ViewModels
{
    public class MTGCardCollectionViewModel : ViewModelBase
  {
    public MTGCardCollectionViewModel(MTGCardCollectionModel model)
    {
      Model = model;
      model.Cards.CollectionChanged += Model_CollectionChanged;

      Charts = new MTGCardModelChart[]{
        new CMCChart(model.Cards),
        new SpellTypeChart(model.Cards)
      };

      AddCommand = new RelayCommand<MTGCardModel>((model) => Add(model));
      RemoveCommand = new RelayCommand<MTGCardModel>((model) => Remove(model));
      ResetCommand = new RelayCommand(() => Reset());
      DeleteDeckFileCommand = new RelayCommand(() => DeleteDeckFile());
      ChangeSortDirectionCommand = new RelayCommand<string>((dir) => ChangeSortDirection(dir));
      ChangeSortPropertyCommand = new RelayCommand<string>((prop) => ChangeSortProperty(prop));
      SaveCommand = new RelayCommand<string>((name) => Save(name));
      LoadCommand = new RelayCommand<string>(async (name) => await LoadAsync(name));
      FetchCardsCommand = new RelayCommand<string>(async (query) => await FetchCards(query));
    }

    private MTGCardCollectionModel Model { get; }
    public ObservableCollection<MTGCardViewModel> CardViewModels { get; } // Synced to Model.Cards
    public MTGCardModelChart[] Charts { get; }

    private bool hasFile;
    private bool isLoading;

    public int TotalCount => Model.TotalCount;
    public SortProperty CollectionSortProperty { get; private set; } = SortProperty.Name;
    public SortDirection CollectionSortDirection { get; private set; } = SortDirection.ASC;
    public bool UnsavedChanges { get; private set; }
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

    public ICommand AddCommand { get; }
    public ICommand RemoveCommand { get; }
    public ICommand ResetCommand { get; }
    public ICommand DeleteDeckFileCommand { get; }
    public ICommand ChangeSortDirectionCommand { get; }
    public ICommand ChangeSortPropertyCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand LoadCommand { get; }
    public ICommand FetchCardsCommand { get; }

    private void Add(MTGCardModel model)
    {
      Model.Add(model);
      Model.SortCollection(CollectionSortDirection, CollectionSortProperty);
      UnsavedChanges = true;
    }
    private void Remove(MTGCardModel model)
    {
      Model.Remove(model);
      UnsavedChanges = true;
    }
    private void Remove(string id)
    {
      Model.Remove(Model.Cards.First(x => x.Info.Id == id));
      UnsavedChanges = true;
    }
    private void Reset()
    {
      Model.Reset();
      UnsavedChanges = true;
      HasFile = false;
    }
    private void DeleteDeckFile()
    {
      if (Model.Name != "")
        IO.DeleteFile($"{IO.CollectionsPath}{Model.Name}.json");
      if (ResetCommand.CanExecute(null)) ResetCommand.Execute(null);
    }
    private void ChangeSortDirection(string dir)
    {
      if (Enum.TryParse(dir, true, out SortDirection sortDirection))
      {
        CollectionSortDirection = sortDirection;
        Model.SortCollection(CollectionSortDirection, CollectionSortProperty);
      }
    }
    private void ChangeSortProperty(string prop)
    {
      if (Enum.TryParse(prop, true, out SortProperty sortProperty))
      {
        CollectionSortProperty = sortProperty;
        Model.SortCollection(CollectionSortDirection, CollectionSortProperty);
      }
    }
    private void Save(string name)
    {
      Model.Rename(name);
      IO.WriteTextToFile($"{IO.CollectionsPath}/{name}.json",
        JsonSerializer.Serialize(Model.Cards.Select(x => new
        {
          x.Info.Id,
          x.Count
        })));
      UnsavedChanges = false;
      HasFile = true;
    }
    private async Task LoadAsync(string name)
    {
      IsLoading = true;
      if (ResetCommand.CanExecute(null)) { ResetCommand.Execute(null); }
      Model.Rename(name);
      var ids = JsonNode.Parse(IO.ReadTextFromFile(
        $"{IO.CollectionsPath}/{name}.json")).AsArray();

      var IdObjects = ids.Select(x => new
      {
        Id = x["Id"].GetValue<string>(),
        Count = x["Count"].GetValue<int>()
      });


      // Scryfall API allows to fetch only 75 cards at once
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

        var cardCollection = await ScryfallAPI.FetchCollection(identifiersJson);
        foreach (var item in cardCollection)
        {
          // Update counts to the fetched cards
          item.Count = chunk.First(x => x.Id == item.Info.Id).Count;

          Model.Add(item);
        }
      }

      Model.SortCollection(CollectionSortDirection, CollectionSortProperty);
      UnsavedChanges = false;
      HasFile = true;
      IsLoading = false;
    }
    /// <summary>
    /// Fetches cards from API and changes cards to the fetched cards
    /// </summary>
    /// <param name="query">API query parameters</param>
    /// <returns></returns>
    private async Task FetchCards(string query)
    {
      IsLoading = true;
      Reset();
      if (query != "")
      {
        var cards = await ScryfallAPI.FetchCards(query);
        foreach (MTGCardModel card in cards)
        {
          // TODO: dont sort, add range
          Add(card);
        }
      }
      IsLoading = false;
    }

    private void Model_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      // Sync Model.Cards and CardViewModels
      MTGCardModel modelCard;

      switch (e.Action)
      {
        case NotifyCollectionChangedAction.Add:
          modelCard = e.NewItems[0] as MTGCardModel;
          var newViewModelCard = new MTGCardViewModel(modelCard) { RemoveRequestCommand = new RelayCommand<string>((id) => Remove(id)) };
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
    }
    private void ModelCard_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if(e.PropertyName == nameof(MTGCardModel.Count))
      {
        OnPropertyChanged(nameof(TotalCount));
        UnsavedChanges = true;
      }
    }
  }
}
