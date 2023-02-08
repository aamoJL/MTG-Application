using System.ComponentModel;
using System.Linq;
using System.Text.Json;
using System.Collections.Specialized;
using MTGApplication.Models;
using CommunityToolkit.Mvvm.Input;
using System;
using static MTGApplication.Models.MTGCardCollectionModel;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace MTGApplication.ViewModels
{
  public partial class MTGCardCollectionViewModel : ViewModelBase
  {
    public MTGCardCollectionViewModel(MTGCardCollectionModel model)
    {
      Model = model;
      //model.Cards.CollectionChanged += Model_CollectionChanged;
      //model.PropertyChanged += Model_PropertyChanged;
    }

    protected virtual MTGCardCollectionModel Model { get; }
    public ObservableCollection<MTGCardViewModel> CardViewModels { get; } = new(); // Synced to Model.Cards

    //protected virtual void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
    //{
    //  //switch (e.PropertyName)
    //  //{
    //  //  case nameof(Model.Cards): OnPropertyChanged(nameof(CardModels)); break;
    //  //  case nameof(Model.TotalCount):
    //  //    OnPropertyChanged(nameof(TotalCount));
    //  //    //if (SelectedSortProperty == SortProperty.Count) { Model.SortCollection(SelectedSortDirection, SelectedSortProperty); } break;
    //  //  default: break;
    //  }
    //}
    ////protected virtual void Model_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    ////{
    ////  // Sync Model.Cards and CardViewModels
    ////  MTGCard modelCard;

    ////  //switch (e.Action)
    ////  //{
    ////  //  case NotifyCollectionChangedAction.Add:
    ////  //    modelCard = e.NewItems[0] as MTGCard;
    ////  //    //var newViewModelCard = new MTGCardViewModel(modelCard) { RemoveRequestCommand = new RelayCommand(() => RemoveModel(modelCard)) };
    ////  //    //CardViewModels.Add(newViewModelCard);
    ////  //    break;
    ////  //  case NotifyCollectionChangedAction.Remove:
    ////  //    CardViewModels.RemoveAt(e.OldStartingIndex);
    ////  //    break;
    ////  //  case NotifyCollectionChangedAction.Reset:
    ////  //    CardViewModels.Clear();
    ////  //    break;
    ////  //  case NotifyCollectionChangedAction.Move:
    ////  //    CardViewModels.Move(e.OldStartingIndex, e.NewStartingIndex);
    ////  //    break;
    ////  //  case NotifyCollectionChangedAction.Replace:
    ////  //  default:
    ////  //    break;
    ////  //}
    ////}

    ////public int TotalCount => Model.TotalCount;
    ////public ObservableCollection<MTGCard> CardModels => Model.Cards;

    //// ViewModel properties
    //[ObservableProperty]
    //protected bool isBusy;
    //[ObservableProperty]
    //protected SortProperty selectedSortProperty = SortProperty.Name;
    //[ObservableProperty]
    //protected SortDirection selectedSortDirection = SortDirection.ASC;

    //[RelayCommand]
    //public void SortByDirection(string dir)
    //{
    //  if (Enum.TryParse(dir, true, out SortDirection sortDirection))
    //  {
    //    //SelectedSortDirection = sortDirection;
    //    //Model.SortCollection(SelectedSortDirection, SelectedSortProperty);
    //  }
    //}
    //[RelayCommand]
    //public void SortByProperty(string prop)
    //{
    //  if (Enum.TryParse(prop, true, out SortProperty sortProperty))
    //  {
    //    SelectedSortProperty = sortProperty;
    //    Model.SortCollection(SelectedSortDirection, SelectedSortProperty);
    //  }
    //}
    //[RelayCommand]
    //public void AddViewModel(MTGCardViewModel newViewModel)
    //{
    //  //var newModel = new MTGCard(newViewModel.CardInfo, newViewModel.Count);
    //  //AddModel(newModel);
    //}
    //public void AddModel(MTGCard newModel)
    //{
    //  Model.Add(newModel);
    //  //Model.SortCollection(SelectedSortDirection, SelectedSortProperty);
    //}
    //public void RemoveModel(MTGCard model)
    //{
    //  Model.Remove(model);
    //}
    //public void RemoveViewModel(MTGCardViewModel viewModel)
    //{
    //  //if (string.IsNullOrEmpty(viewModel?.CardInfo.ScryfallId)) { return; }
    //  //RemoveModel(Model.Cards.First(x => x.Info.ScryfallId == viewModel.CardInfo.ScryfallId));
    //}
    //public virtual void Clear() => Model.Clear();

    ///// <summary>
    ///// Imports cards from formatted text.
    ///// <code>Format example:
    ///// 4 Black Lotus
    ///// Mox Ruby</code>
    ///// </summary>
    //public virtual async Task ImportFromStringAsync(string text)
    //{
    //  //IsBusy = true;

    //  var lines = text.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

    //  var IdObjects = lines.Select(line =>
    //  {
    //    // Format: {Count (optional)} {Name}
    //    // Stops at '/' so only the first name will be used for multiface cards
    //    var regexGroups = new Regex("(?:^[\\s]*(?<Count>[0-9]*(?=\\s)){0,1}\\s*(?<Name>[\\s\\S][^/]*))");
    //    var match = regexGroups.Match(line);

    //    var countMatch = match.Groups["Count"]?.Value;
    //    var nameMatch = match.Groups["Name"]?.Value;

    //    if (string.IsNullOrEmpty(nameMatch)) { return null; }

    //    return new
    //    {
    //      Count = !string.IsNullOrEmpty(countMatch) ? int.Parse(countMatch) : 1,
    //      Name = nameMatch.Trim(),
    //    };
    //  });

    //  List<Task<MTGCard[]>> chunkTasks = new(IdObjects.Chunk(App.CardAPI.MaxFetchIdentifierCount).Select(chunk => Task.Run(async () =>
    //  {
    //    // Fetch cards in chunks
    //    var identifiersJson = string.Empty;

    //    using (var stream = new MemoryStream())
    //    {
    //      await JsonSerializer.SerializeAsync(stream, new
    //      {
    //        identifiers = chunk.Select(x => new
    //        {
    //          name = x.Name
    //        })
    //      });
    //      stream.Position = 0;
    //      using var reader = new StreamReader(stream);
    //      identifiersJson = await reader.ReadToEndAsync();
    //    }

    //    MTGCard[] fetchedCards = null; //await App.CardAPI.FetchCollectionAsync(identifiersJson);
    //    foreach (var item in fetchedCards)
    //    {
    //      //Update counts to the fetched cards
    //      //var found = chunk.FirstOrDefault(x => item.Info.CardFaces[0].Name == x.Name);
    //      //if (found != null) { item.Count = found.Count; }
    //    }

    //    return fetchedCards;
    //  })));

    //  var fetchedCardLists = await Task.WhenAll(chunkTasks);

    //  foreach (var list in fetchedCardLists)
    //  {
    //    foreach (var card in list)
    //    {
    //      Model.Add(card);
    //    }
    //  }

    //  //IsBusy = false;
    //}
  }
}
