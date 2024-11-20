using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MTGApplication.Features.CardCollectionEditor.CardCollection.Models;
using MTGApplication.Features.CardCollectionEditor.CardCollection.Services;
using MTGApplication.Features.CardCollectionEditor.CardCollection.Services.Converters;
using MTGApplication.Features.CardCollectionEditor.CardCollectionList.Models;
using MTGApplication.General.Services.Databases.Repositories;
using MTGApplication.General.Services.Databases.Repositories.CardCollectionRepository;
using MTGApplication.General.Services.Databases.Repositories.CardCollectionRepository.Models;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplication.General.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using static MTGApplication.Features.CardCollectionEditor.CardCollection.UseCases.CardCollectionEditorViewModelCommands;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplication.Features.CardCollectionEditor.CardCollection.ViewModels;

public partial class CardCollectionViewModel(MTGCardCollection model, MTGCardImporter importer) : ObservableObject, IWorker, ISavable
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
      OnPropertyChanged();
    }
  }
  public ObservableCollection<MTGCardCollectionList> CollectionLists => Model.CollectionLists;

  public bool IsBusy { get; set { field = value; OnPropertyChanged(); } }
  public bool HasUnsavedChanges { get; set { field = value; OnPropertyChanged(); } }

  private MTGCardCollection Model { get; } = model;

  public Func<Task> OnDelete { get; init; }
  // List change actions are used to update selected list on CardCollectionPage, because Combobox does not work with collection change events
  public Func<MTGCardCollectionList, Task> OnListAdded { get; init; }
  public Func<MTGCardCollectionList, Task> OnListRemoved { get; init; }

  public IAsyncRelayCommand SaveCollectionCommand => field ??= new ConfirmSaveCollection(this).Command;
  public IAsyncRelayCommand DeleteCollectionCommand => field ??= new ConfirmDeleteCollection(this).Command;
  public IAsyncRelayCommand NewListCommand => field ??= new ConfirmNewList(this).Command;
  public IAsyncRelayCommand<MTGCardCollectionList> DeleteListCommand => field ??= new ConfirmDeleteList(this).Command;

  public MTGCardCollectionDTO AsDTO() => CardCollectionToDTOConverter.Convert(Model);
}
