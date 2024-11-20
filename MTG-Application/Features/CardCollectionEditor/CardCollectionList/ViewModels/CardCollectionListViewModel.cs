using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MTGApplication.Features.CardCollectionEditor.CardCollectionList.Models;
using MTGApplication.Features.CardCollectionEditor.CardCollectionList.Services;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplication.General.Services.IOServices;
using MTGApplication.General.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using static MTGApplication.Features.CardCollectionEditor.CardCollectionList.UseCases.CardCollectionEditorViewModelCommands;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplication.Features.CardCollectionEditor.CardCollectionList.ViewModels;

public partial class CardCollectionListViewModel : ObservableObject, ISavable, IWorker
{
  public CardCollectionListViewModel(MTGCardCollectionList model, MTGCardImporter importer, Func<string, bool> existsValidation = null)
  {
    Model = model ?? new();

    Importer = importer;
    ExistsValidation = existsValidation;

    QueryCardsViewModel = new(OwnedCards, importer);
  }

  public MTGCardImporter Importer { get; }
  public QueryCardsViewModel QueryCardsViewModel { get; }
  public CardCollectionListConfirmers Confirmers { get; set; } = new();
  public Notifier Notifier { get; set; } = new();
  public ClipboardService ClipboardService { get; set; } = new();
  public IWorker Worker => this;

  public string Name
  {
    get => Model.Name;
    set
    {
      if (Model.Name == value) return;

      Model.Name = value;
      OnPropertyChanged(nameof(Name));
    }
  }
  public string Query
  {
    get => Model.SearchQuery;
    set
    {
      if (Model.SearchQuery == value) return;

      Model.SearchQuery = value;
      OnPropertyChanged(nameof(Query));
    }
  }
  public ObservableCollection<CardCollectionMTGCard> OwnedCards => Model.Cards;

  [ObservableProperty] public partial bool HasUnsavedChanges { get; set; }
  [ObservableProperty] public partial bool IsBusy { get; set; }

  public Func<string, bool> ExistsValidation { get; }

  private MTGCardCollectionList Model { get; }

  public IAsyncRelayCommand EditListCommand => field ??= new EditList(this).Command;
  public IAsyncRelayCommand ImportCardsCommand => field ??= new ImportCards(this).Command;
  public IAsyncRelayCommand ExportCardsCommand => field ??= new ExportCards(this).Command;
  public IAsyncRelayCommand<CardCollectionMTGCard> ShowCardPrintsCommand => field ??= new ShowCardPrints(this).Command;
  public IRelayCommand<CardCollectionMTGCard> SwitchCardOwnershipCommand => field ??= new SwitchCardOwnership(this).Command;

  public async Task UpdateQueryCards() => await Worker.DoWork(QueryCardsViewModel.UpdateQueryCards(Model.SearchQuery));
}