﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MTGApplication.Features.CardCollection.Services;
using MTGApplication.General.Services.Databases.Repositories;
using MTGApplication.General.Services.Databases.Repositories.CardCollectionRepository;
using MTGApplication.General.Services.Databases.Repositories.CardCollectionRepository.Models;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplication.General.Services.IOServices;
using MTGApplication.General.ViewModels;
using System.Linq;
using System.Threading.Tasks;
using static MTGApplication.Features.CardCollection.UseCases.CardCollectionViewModelCommands;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplication.Features.CardCollection;

public partial class CardCollectionViewModel(MTGCardImporter importer) : ViewModelBase, IWorker, ISavable
{
  public QueryCardsViewModel QueryCardsViewModel { get; } = new(importer);
  public CardCollectionConfirmers Confirmers { get; init; } = new();
  public Notifier Notifier { get; init; } = new();
  public ClipboardService ClipboardService { get; init; } = new();
  public IRepository<MTGCardCollectionDTO> Repository { get; init; } = new CardCollectionDTORepository();
  public MTGCardImporter Importer { get; } = importer;

  [ObservableProperty] private MTGCardCollection collection = new();
  [ObservableProperty] private MTGCardCollectionList selectedList;
  [ObservableProperty] private bool isBusy;
  [ObservableProperty] private bool hasUnsavedChanges;

  public int SelectedListCardCount => SelectedList?.Cards.Count ?? 0;
  public IWorker Worker => this;

  public IAsyncRelayCommand<ISavable.ConfirmArgs> ConfirmUnsavedChangesCommand => new ConfirmUnsavedChanges(this).Command;
  public IAsyncRelayCommand NewCollectionCommand => new NewCollection(this).Command;
  public IAsyncRelayCommand OpenCollectionCommand => new OpenCollection(this).Command;
  public IAsyncRelayCommand SaveCollectionCommand => new SaveCollection(this).Command;
  public IAsyncRelayCommand DeleteCollectionCommand => new DeleteCollection(this).Command;
  public IAsyncRelayCommand NewListCommand => new NewList(this).Command;
  public IAsyncRelayCommand EditListCommand => new EditList(this).Command;
  public IAsyncRelayCommand DeleteListCommand => new DeleteList(this).Command;
  public IAsyncRelayCommand ImportCardsCommand => new ImportCards(this).Command;
  public IAsyncRelayCommand ExportCardsCommand => new ExportCards(this).Command;
  public IAsyncRelayCommand<MTGCardCollectionList> SelectListCommand => new SelectList(this).Command;
  public IAsyncRelayCommand<CardCollectionMTGCard> ShowCardPrintsCommand => new ShowCardPrints(this).Command;
  public IRelayCommand<CardCollectionMTGCard> SwitchCardOwnershipCommand => new SwitchCardOwnership(this).Command;

  public async Task SetCollection(MTGCardCollection collection)
  {
    Collection = collection;
    HasUnsavedChanges = false;

    await SelectListCommand.ExecuteAsync(Collection.CollectionLists.FirstOrDefault());
  }
}