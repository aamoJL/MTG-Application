using CommunityToolkit.Mvvm.Input;
using MTGApplication.Features.CardCollectionEditor.Models;
using MTGApplication.Features.CardCollectionEditor.UseCases;
using MTGApplication.Features.CardCollectionEditor.ViewModels.Collection;
using MTGApplication.General.Services.Databases.Repositories;
using MTGApplication.General.Services.Databases.Repositories.CardCollectionRepository;
using MTGApplication.General.Services.Databases.Repositories.CardCollectionRepository.Models;
using MTGApplication.General.Services.Exporters;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplication.General.Services.NotificationService.UseCases;
using MTGApplication.General.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplication.Features.CardCollectionEditor.ViewModels.EditorPage;

public partial class CardCollectionEditorPageViewModel : ViewModelBase
{
  public Worker Worker { get; init; } = new();
  public IRepository<MTGCardCollectionDTO> Repository { private get; init; } = new CardCollectionDTORepository();
  public IMTGCardImporter Importer { private get; init; } = App.MTGCardImporter;
  public Notifier Notifier { private get; init; } = new();
  public IExporter<string> Exporter { private get; init; } = new ClipboardExporter();
  public CollectionEditorPageConfirmers Confirmers { private get; init; } = new();

  public string CollectionName => CollectionViewModel.CollectionName;
  public CardCollectionViewModel CollectionViewModel
  {
    get => field ??= CollectionViewModel = CollectionViewModelFactory.Build(new());
    private set
    {
      var nameChanged = field?.CollectionName != value?.CollectionName;

      if (field != null)
        field.PropertyChanged -= CollectionViewModel_PropertyChanged;

      SetProperty(ref field, value);

      if (field != null)
        field.PropertyChanged += CollectionViewModel_PropertyChanged;

      // Visual state trigger will not work if the OnPropertyChanged(nameof(CollectionName))
      //  is called when the old name was the same as the new name.
      if (nameChanged)
        OnPropertyChanged(nameof(CollectionName));
    }
  }

  private CardCollectionViewModel.Factory CollectionViewModelFactory
  {
    get => field ??= new()
    {
      Worker = Worker,
      Repository = Repository,
      Notifier = Notifier,
      Exporter = Exporter,
      Importer = Importer,
      CollectionConfirmers = Confirmers.CollectionConfirmers,
      OnCollectionDeleted = OnCollectionDeleted,
    };
  }

  [RelayCommand]
  private async Task NewCollection()
  {
    var saveArgs = new SaveStatus.ConfirmArgs();

    if (CollectionViewModel.SaveStatus.HasUnsavedChanges)
      await CollectionViewModel.SaveUnsavedChangesCommand.ExecuteAsync(saveArgs);

    if (saveArgs.Cancelled)
      return;

    await ChangeCollection(new());
  }

  [RelayCommand]
  private async Task OpenCollection()
  {
    try
    {
      var saveArgs = new SaveStatus.ConfirmArgs();

      await CollectionViewModel.SaveUnsavedChangesCommand.ExecuteAsync(saveArgs);

      if (saveArgs.Cancelled)
        return;

      var collectionNames = await Worker.DoWork(new FetchCardCollectionNames(Repository).Execute);

      if ((await Confirmers.ConfirmCollectionOpen(Confirmations.GetLoadCollectionConfirmation(collectionNames)))
        is not string loadName || string.IsNullOrEmpty(loadName))
      {
        return; // Cancel
      }

      if ((await Worker.DoWork(new FetchCardCollection(Repository, Importer).Execute(loadName))) is MTGCardCollection loadedCollection)
      {
        await ChangeCollection(loadedCollection);

        new ShowNotification(Notifier).Execute(Notifications.OpenCollectionSuccess);
      }
      else
        new ShowNotification(Notifier).Execute(Notifications.OpenCollectionError);
    }
    catch (Exception e)
    {
      new ShowNotification(Notifier).Execute(new(NotificationType.Error, e.Message));
    }
  }

  [RelayCommand]
  private async Task ChangeCollection(MTGCardCollection collection)
  {
    CollectionViewModel = CollectionViewModelFactory.Build(collection);

    await CollectionViewModel.ChangeListCommand.ExecuteAsync(CollectionViewModel.CollectionListViewModels.FirstOrDefault());
  }

  private async Task OnCollectionDeleted() => await ChangeCollection(new());

  private void CollectionViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
  {
    switch (e.PropertyName)
    {
      case nameof(CardCollectionViewModel.CollectionName):
        OnPropertyChanged(nameof(CollectionName));
        break;
    }
  }
}