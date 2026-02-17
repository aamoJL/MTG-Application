using CommunityToolkit.Mvvm.Input;
using MTGApplication.Features.CardCollectionEditor.Models;
using MTGApplication.Features.CardCollectionEditor.UseCases;
using MTGApplication.Features.CardCollectionEditor.ViewModels.CollectionCard;
using MTGApplication.General.Extensions;
using MTGApplication.General.Models;
using MTGApplication.General.Services.ConfirmationService;
using MTGApplication.General.Services.Exporters;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplication.General.Services.NotificationService.UseCases;
using MTGApplication.General.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplication.Features.CardCollectionEditor.ViewModels.CollectionList;

public partial class CardCollectionListViewModel : ViewModelBase
{
  public CardCollectionListViewModel(MTGCardCollectionList list)
  {
    Model = list;

    Model.PropertyChanged += Model_PropertyChanged;
    Model.Cards.CollectionChanged += Cards_CollectionChanged;
  }

  public Worker Worker { get; init; } = new();
  public SaveStatus SaveStatus { get; init; } = new();
  public IMTGCardImporter Importer { private get; init; } = App.MTGCardImporter;
  public Notifier Notifier { private get; init; } = new();
  public IExporter<string> Exporter { private get; init; } = new ClipboardExporter();
  public CollectionListConfirmers Confirmers { private get; init; } = new();
  public Func<string, bool> NameValidator { private get => field ?? throw new NotImplementedException(); init; }

  public string Name => Model.Name;
  public string Query => Model.SearchQuery;
  public ReadOnlyObservableCollection<MTGCard> Cards => field ??= new(Model.Cards);
  public IncrementalLoadingCardCollection<CardCollectionMTGCardViewModel> QueryCards
  {
    get => field ??= QueryCards = CreateQueryCollection([], string.Empty, 0);
    private set
    {
      if (field != null)
      {
        field.Collection.CollectionChanged -= QueryCards_CollectionChanged;

        foreach (var item in field.Collection)
          item.PropertyChanged -= QueryCard_PropertyChanged;
      }

      SetProperty(ref field, value);

      if (field != null)
      {
        field.Collection.CollectionChanged += QueryCards_CollectionChanged;

        foreach (var item in field.Collection)
          item.PropertyChanged += QueryCard_PropertyChanged;
      }
    }
  }

  public Func<MTGCardCollectionList, Task>? OnDelete { get; init; }

  private MTGCardCollectionList Model { get; }
  private CardCollectionMTGCardViewModel.Factory CardViewModelFactory
  {
    get => field ??= new()
    {
      Worker = Worker,
      CardConfirmers = Confirmers.CardConfirmers,
      Importer = Importer,
      Notifier = Notifier,
      IsOwned = (card) => Cards.FirstOrDefault(x => x.Info.ScryfallId == card.Info.ScryfallId) != null,
    };
  }

  [RelayCommand]
  private async Task EditList()
  {
    try
    {
      if (await Confirmers.ConfirmEditList(Confirmations.GetEditCollectionListConfirmation((Name, Query)))
        is not (string newName, string newQuery))
      {
        return; // Cancel
      }

      if (string.IsNullOrEmpty(newName)) throw new(Notifications.EditListNameError.Message);
      if (string.IsNullOrEmpty(newQuery)) throw new(Notifications.EditListQueryError.Message);
      if (Name != newName && NameValidator.Invoke(newName) != true) throw new(Notifications.EditListExistsError.Message);

      await Worker.DoWork(async () =>
      {
        if (newQuery != Query)
        {
          // Fetch new query cards and remove cards that are not in the new query
          //  from the owned cards if the user accepts the conflict
          var found = (await new FetchCardsWithQuery(Importer).Execute(Query)).Found;

          var excludedCards = Cards
            .ExceptBy(found.Select(f => f.Info.ScryfallId), o => o.Info.ScryfallId)
            .ToList();

          if (excludedCards.Count != 0 && await Confirmers.ConfirmEditQueryConflict(Confirmations.GetEditCollectionListQueryConflictConfirmation(excludedCards.Count))
            is not ConfirmationResult.Yes)
          {
            return; // Cancel
          }

          foreach (var item in excludedCards)
            Model.Cards.Remove(item);
        }

        Model.SearchQuery = newQuery;
        Model.Name = newName;

        new ShowNotification(Notifier).Execute(Notifications.EditListSuccess);
      });
    }
    catch (Exception e)
    {
      new ShowNotification(Notifier).Execute(new(NotificationType.Error, $"Error: {e.Message}"));
    }
  }

  [RelayCommand]
  private async Task DeleteList()
  {
    try
    {
      if (await Confirmers.ConfirmListDelete(Confirmations.GetDeleteCollectionListConfirmation(Name))
        is not ConfirmationResult.Yes)
        return; // Cancel

      if (OnDelete != null)
        await OnDelete(Model);
    }
    catch
    {
      new ShowNotification(Notifier).Execute(Notifications.DeleteListError);
    }
  }

  [RelayCommand]
  private async Task ImportCards()
  {
    try
    {
      var importText = await Confirmers.ConfirmCardImport(Confirmations.GetImportCardsConfirmation());

      if (string.IsNullOrEmpty(importText))
        return; // Cancel

      var (importResult, newCards) = await Worker.DoWork(async () =>
      {
        // Fetch imported cards and add the cards that are included in the query but not in the owned cards
        var importTask = Task.Run(() => new FetchCardsWithImportText(Importer).Execute(importText));
        var queryTask = Task.Run(() => new FetchCardsWithQuery(Importer).Execute(Query));

        await Task.WhenAll(importTask, queryTask);

        if (importTask.IsFaulted) throw importTask.Exception;
        if (queryTask.IsFaulted) throw queryTask.Exception;

        var addedCards = importTask.Result.Found.Select(f => new MTGCard(f.Info))
          .IntersectBy(queryTask.Result.Found.Select(c => c.Info.ScryfallId), vm => vm.Info.ScryfallId)
          .ExceptBy(Cards.Select(o => o.Info.ScryfallId), f => f.Info.ScryfallId)
          .DistinctBy(x => x.Info.ScryfallId)
          .ToList();

        return (importTask.Result, addedCards);
      });

      if (importResult.Found.Length == 0)
        throw new(Notifications.ImportCardsError.Message);

      foreach (var card in newCards)
        Model.Cards.Add(card);

      new ShowNotification(Notifier).Execute(Notifications.ImportCardsSuccessOrWarning(
        added: newCards.Count,
        skipped: importResult.Found.Length - newCards.Count,
        notFound: importResult.NotFoundCount));
    }
    catch (Exception e)
    {
      new ShowNotification(Notifier).Execute(new(NotificationType.Error, $"Error: {e.Message}"));
    }
  }

  [RelayCommand]
  private async Task ExportCards()
  {
    try
    {
      var exportString = string.Join(Environment.NewLine, Cards.Select(x => x.Info.ScryfallId));

      if (await Confirmers.ConfirmCardExport(Confirmations.GetExportCardsConfirmation(exportString))
        is not string response || string.IsNullOrEmpty(response))
      {
        return; // Cancel
      }

      new ExportText(Exporter).Execute(response);

      new ShowNotification(Notifier).Execute(Exporter.SuccessNotification);
    }
    catch (Exception e)
    {
      new ShowNotification(Notifier).Execute(new(NotificationType.Error, $"Error: {e.Message}"));
    }
  }

  [RelayCommand]
  private async Task Refresh()
  {
    try
    {
      await Worker.DoWork(async () =>
      {
        if ((await new FetchCardsWithQuery(Importer) { Pagination = true }.Execute(Query)) is not CardImportResult fetchResult)
          return;

        QueryCards = CreateQueryCollection(
          cards: fetchResult.Found.Select(x => CardViewModelFactory.Build(new(x.Info))),
          nextPage: fetchResult.NextPageUri,
          totalCount: fetchResult.TotalCount);
      });
    }
    catch (Exception e)
    {
      new ShowNotification(Notifier).Execute(new(NotificationType.Error, $"Error: {e.Message}"));
    }
  }

  private IncrementalLoadingCardCollection<CardCollectionMTGCardViewModel> CreateQueryCollection(IEnumerable<CardCollectionMTGCardViewModel> cards, string nextPage, int totalCount)
  {
    var source = new IncrementalCardSource<CardCollectionMTGCardViewModel>(Importer)
    {
      Cards = [.. cards],
      NextPage = nextPage,
      Converter = (item) => CardViewModelFactory.Build(new(item.Info)),
      OnError = (e) => new ShowNotification(Notifier).Execute(new(NotificationType.Error, e.Message)),
    };

    return new(source)
    {
      TotalCardCount = totalCount,
    };
  }

  private void Model_PropertyChanged(object? sender, PropertyChangedEventArgs e)
  {
    switch (e.PropertyName)
    {
      case nameof(MTGCardCollectionList.Name): OnPropertyChanged(nameof(Name)); break;
      case nameof(MTGCardCollectionList.SearchQuery): OnPropertyChanged(nameof(Query)); break;
    }

    SaveStatus.HasUnsavedChanges = true;
  }

  private void Cards_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    => SaveStatus.HasUnsavedChanges = true;

  private void QueryCards_CollectionChanged(object? _, NotifyCollectionChangedEventArgs e)
  {
    foreach (var item in e.AddedItems<CardCollectionMTGCardViewModel>())
      item.PropertyChanged += QueryCard_PropertyChanged;

    foreach (var item in e.RemovedItems<CardCollectionMTGCardViewModel>())
      item.PropertyChanged -= QueryCard_PropertyChanged;
  }

  private void QueryCard_PropertyChanged(object? sender, PropertyChangedEventArgs e)
  {
    if (sender is not CardCollectionMTGCardViewModel card) return;

    if (e.PropertyName == nameof(CardCollectionMTGCardViewModel.IsOwned))
    {
      if (card.IsOwned && Cards.FirstOrDefault(x => x.Info.ScryfallId == card.Info.ScryfallId) == null)
        Model.Cards.Add(new(card.Info));
      else if (!card.IsOwned && Model.Cards.TryFindIndex(x => x.Info.ScryfallId == card.Info.ScryfallId, out var index))
        Model.Cards.RemoveAt(index);
    }
  }
}
