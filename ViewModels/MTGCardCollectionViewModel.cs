﻿using System.ComponentModel;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Collections.Specialized;
using MTGApplication.Models;
using CommunityToolkit.Mvvm.Input;
using System;
using static MTGApplication.Models.MTGCardCollectionModel;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MTGApplication.ViewModels
{
  public partial class MTGCardCollectionViewModel : ViewModelBase
  {
    public MTGCardCollectionViewModel(MTGCardCollectionModel model)
    {
      Model = model;
      model.Cards.CollectionChanged += Model_CollectionChanged;
      model.PropertyChanged += Model_PropertyChanged;
    }

    private MTGCardCollectionModel Model { get; }
    public ObservableCollection<MTGCardViewModel> CardViewModels { get; } = new(); // Synced to Model.Cards

    // Model properties
    public string Name => Model.Name;
    public int TotalCount => Model.TotalCount;
    public ObservableCollection<MTGCardModel> CardModels => Model.Cards;

    // ViewModel properties
    [ObservableProperty]
    private bool isLoading;
    [ObservableProperty]
    private SortProperty selectedSortProperty = SortProperty.Name;
    [ObservableProperty]
    private SortDirection selectedSortDirection = SortDirection.ASC;
    [ObservableProperty]
    private bool hasUnsavedChanges;

    [RelayCommand]
    public void AddAndCombineAndSort(MTGCardModel newModel)
    {
      Model.Add(newModel);
      Model.SortCollection(SelectedSortDirection, SelectedSortProperty);
    }
    [RelayCommand]
    public void RemoveModel(MTGCardModel model)
    {
      Model.Remove(model);
    }
    [RelayCommand]
    public void RemoveViewModel(MTGCardViewModel viewModel)
    {
      if (string.IsNullOrEmpty(viewModel?.CardInfo.Id)) { return; }
      RemoveModel(Model.Cards.First(x => x.Info.Id == viewModel.CardInfo.Id));
    }
    [RelayCommand]
    public void Reset()
    {
      Model.Reset();
      HasUnsavedChanges = false;
    }
    [RelayCommand]
    public void DeleteDeckFile()
    {
      if (Name != "")
        IO.DeleteFile($"{IO.CollectionsPath}{Name}.json");
      Reset();
    }
    [RelayCommand]
    public void ChangeSortDirectionAndSort(string dir)
    {
      if (Enum.TryParse(dir, true, out SortDirection sortDirection))
      {
        SelectedSortDirection = sortDirection;
        Model.SortCollection(SelectedSortDirection, SelectedSortProperty);
      }
    }
    [RelayCommand]
    public void ChangeSortPropertyAndSort(string prop)
    {
      if (Enum.TryParse(prop, true, out SortProperty sortProperty))
      {
        SelectedSortProperty = sortProperty;
        Model.SortCollection(SelectedSortDirection, SelectedSortProperty);
      }
    }
    [RelayCommand]
    public void Save(string name)
    {
      Model.Name = name;
      IO.WriteTextToFile($"{IO.CollectionsPath}/{name}.json",
        JsonSerializer.Serialize(Model.Cards.Select(x => new
        {
          x.Info.Id,
          x.Count
        })));
      HasUnsavedChanges = false;
    }
    [RelayCommand]
    public async Task LoadAsync(string name)
    {
      IsLoading = true;
      Reset();

      try
      {
        var jsonString = await IO.ReadTextFromFileAsync($"{IO.CollectionsPath}/{name}.json");
        var ids = JsonNode.Parse(jsonString).AsArray();

        var IdObjects = ids.Select(x => new
        {
          Id = x["Id"].GetValue<string>(),
          Count = x["Count"].GetValue<int>()
        });

        //Scryfall API allows to fetch only 75 cards at a time
        foreach (var chunk in IdObjects.Chunk(75))
        {
          // Fetch cards in chunks

          var identifiersJson = string.Empty;

          using (var stream = new MemoryStream())
          {
            await JsonSerializer.SerializeAsync(stream, new {
              identifiers = chunk.Select(x => new
              {
                id = x.Id
              })
            });
            stream.Position = 0;
            using var reader = new StreamReader(stream);
            identifiersJson = await reader.ReadToEndAsync();
          }

          var cardCollection = await App.CardAPI.FetchCollectionAsync(identifiersJson);
          foreach (var item in cardCollection)
          {
            // Update counts to the fetched cards
            item.Count = chunk.First(x => x.Id == item.Info.Id).Count;
            Model.Add(item);
          }
        }
        Model.Name = name;
        Model.SortCollection(SelectedSortDirection, SelectedSortProperty);
        HasUnsavedChanges = false;
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
      if (!string.IsNullOrEmpty(query))
      {
        var cards = await App.CardAPI.FetchCards(query);
        foreach (var item in cards)
        {
          Model.Add(item);
        }
      }
      HasUnsavedChanges = false;
      IsLoading = false;
    }

    private void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      switch (e.PropertyName)
      {
        case nameof(Model.Name): OnPropertyChanged(nameof(Name)); break;
        case nameof(Model.Cards): OnPropertyChanged(nameof(Name)); break;
        case nameof(Model.TotalCount):
          if(SelectedSortProperty == SortProperty.Count) { Model.SortCollection(SelectedSortDirection, SelectedSortProperty); }
          OnPropertyChanged(nameof(TotalCount));
          HasUnsavedChanges = true; break;
        default: break;
      }
    }
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
          break;
        case NotifyCollectionChangedAction.Remove:
          CardViewModels.RemoveAt(e.OldStartingIndex);
          break;
        case NotifyCollectionChangedAction.Reset:
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
  }
}
