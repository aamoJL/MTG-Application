using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MTGApplication.Features.CardCollection.Services;
using MTGApplication.Features.CardCollection.Services.Converters;
using MTGApplication.General.Services.Databases.Repositories;
using MTGApplication.General.Services.Databases.Repositories.CardCollectionRepository;
using MTGApplication.General.Services.Databases.Repositories.CardCollectionRepository.Models;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplication.General.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using static MTGApplication.Features.CardCollection.UseCases.CardCollectionEditorViewModelCommands;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplication.Features.CardCollection;

public partial class CardCollectionViewModel(MTGCardCollection model, MTGCardImporter importer) : ViewModelBase, IWorker, ISavable
{
  public MTGCardImporter Importer { get; } = importer;
  public CardCollectionConfirmers Confirmers { get; set; } = new();
  public Notifier Notifier { get; set; } = new();
  public IRepository<MTGCardCollectionDTO> Repository { get; set; } = new CardCollectionDTORepository();
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
  public ObservableCollection<MTGCardCollectionList> CollectionLists => Model.CollectionLists;

  [ObservableProperty] private bool isBusy;
  [ObservableProperty] private bool hasUnsavedChanges;

  private MTGCardCollection Model { get; } = model;

  public Func<Task> OnDelete { get; init; }
  // List change actions are used to update selected list on CardCollectionPage, because Combobox does not work with collection change events
  public Func<MTGCardCollectionList, Task> OnListAdded { get; init; }
  public Func<MTGCardCollectionList, Task> OnListRemoved { get; init; }

  public IAsyncRelayCommand SaveCollectionCommand => (saveCollection ??= new ConfirmSaveCollection(this)).Command;
  public IAsyncRelayCommand DeleteCollectionCommand => (deleteCollection ??= new ConfirmDeleteCollection(this)).Command;
  public IAsyncRelayCommand NewListCommand => (newList ??= new ConfirmNewList(this)).Command;
  public IAsyncRelayCommand<MTGCardCollectionList> DeleteListCommand => (deleteList ??= new ConfirmDeleteList(this)).Command;

  private ConfirmSaveCollection saveCollection;
  private ConfirmDeleteCollection deleteCollection;
  private ConfirmNewList newList;
  private ConfirmDeleteList deleteList;

  public MTGCardCollectionDTO AsDTO() => CardCollectionToDTOConverter.Convert(Model);
}
