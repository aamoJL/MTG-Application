using CommunityToolkit.Mvvm.Input;
using MTGApplication.Features.CardCollectionEditor.Models;
using MTGApplication.Features.CardCollectionEditor.UseCases;
using MTGApplication.Features.CardCollectionEditor.ViewModels.CollectionList;
using MTGApplication.General.Extensions;
using MTGApplication.General.Services.ConfirmationService;
using MTGApplication.General.Services.Databases.Repositories;
using MTGApplication.General.Services.Databases.Repositories.CardCollectionRepository;
using MTGApplication.General.Services.Databases.Repositories.CardCollectionRepository.Models;
using MTGApplication.General.Services.Databases.Repositories.CardCollectionRepository.UseCases;
using MTGApplication.General.Services.Exporters;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplication.General.Services.NotificationService.UseCases;
using MTGApplication.General.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplication.Features.CardCollectionEditor.ViewModels.Collection;

public partial class CardCollectionViewModel : ViewModelBase
{
  public CardCollectionViewModel(MTGCardCollection collection)
  {
    Model = collection;

    Model.PropertyChanged += Model_PropertyChanged;
    Model.CollectionLists.CollectionChanged += Model_CollectionLists_CollectionChanged;
  }

  public Worker Worker { get; init; } = new();
  public SaveStatus SaveStatus { get; init; } = new();
  public IRepository<MTGCardCollectionDTO> Repository { private get; init; } = new CardCollectionDTORepository();
  public IMTGCardImporter Importer { private get; init; } = App.MTGCardImporter;
  public Notifier Notifier { private get; init; } = new();
  public IExporter<string> Exporter { private get; init; } = new ClipboardExporter();
  public CollectionConfirmers Confirmers { private get; init; } = new();

  public string CollectionName => Model.Name;
  public ObservableCollection<CardCollectionListViewModel> CollectionListViewModels => field ??= [.. Model.CollectionLists.Select(ListViewModelFactory.Build)];
  public CardCollectionListViewModel? ListViewModel
  {
    get;
    private set => SetProperty(ref field, value);
  }

  public Func<Task>? OnDeleted { get; set; }

  private MTGCardCollection Model { get; }
  private CardCollectionListViewModel.Factory ListViewModelFactory
  {
    get => field ??= new()
    {
      Worker = Worker,
      SaveStatus = SaveStatus,
      Exporter = Exporter,
      CollectionListConfirmers = Confirmers.CollectionListConfirmers,
      Importer = Importer,
      Notifier = Notifier,
      NameValidator = (name) => Model.CollectionLists.Select(x => x.Name).Contains(name),
      OnListDelete = OnListDelete
    };
  }

  [RelayCommand]
  private async Task SaveUnsavedChanges(SaveStatus.ConfirmArgs? args)
  {
    if (args == null || args.Cancelled || !SaveStatus.HasUnsavedChanges) return;

    try
    {
      switch (await Confirmers.ConfirmUnsavedChanges(Confirmations.GetSaveUnsavedChangesConfirmation(CollectionName)))
      {
        case ConfirmationResult.Yes:
          await SaveCollection();
          args.Cancelled = SaveStatus.HasUnsavedChanges;
          break;
        case ConfirmationResult.Cancel:
          args.Cancelled = true;
          break;
      }
    }
    catch (Exception e)
    {
      args.Cancelled = true;

      new ShowNotification(Notifier).Execute(new(NotificationType.Error, $"Error: {e.Message}"));
    }
  }

  [RelayCommand]
  private async Task SaveCollection()
  {
    try
    {
      var oldName = CollectionName;
      var overrideOld = false;

      var saveName = await Confirmers.ConfirmCollectionSave(Confirmations.GetSaveCollectionConfirmation(oldName));

      if (saveName == null)
        return;

      if (saveName == string.Empty)
        throw new InvalidOperationException("Invalid name");

      // Override confirmation
      if (saveName != oldName && await new CardCollectionDTOExists(Repository).Execute(saveName))
      {
        if (await Confirmers.ConfirmCollectionSaveOverride(Confirmations.GetOverrideCollectionConfirmation(saveName))
          is ConfirmationResult.Yes)
        {
          overrideOld = true;
        }
        else return; // Cancel
      }

      if (await Worker.DoWork(new SaveCardCollection(Repository).Execute(Model, saveName, overrideOld)))
      {
        Model.Name = saveName;
        SaveStatus.HasUnsavedChanges = false;

        new ShowNotification(Notifier).Execute(Notifications.SaveCollectionSuccess);
      }
      else
        throw new(Notifications.SaveCollectionError.Message);
    }
    catch (Exception e)
    {
      new ShowNotification(Notifier).Execute(new(NotificationType.Error, $"Error: {e.Message}"));
    }
  }

  [RelayCommand(CanExecute = nameof(CanDelete))]
  private async Task DeleteCollection()
  {
    try
    {
      if (!CanDelete())
        throw new ArgumentException("Name should not be empty", nameof(CollectionName));

      if (await Confirmers.ConfirmCollectionDelete(Confirmations.GetDeleteCollectionConfirmation(CollectionName))
        is not ConfirmationResult.Yes)
      {
        return; // Cancel
      }

      if (await Worker.DoWork(new DeleteCardCollection(Repository).Execute(Model)))
      {
        if (OnDeleted != null)
          await OnDeleted();

        new ShowNotification(Notifier).Execute(Notifications.DeleteCollectionSuccess);
      }
      else
        throw new(Notifications.DeleteCollectionError.Message);
    }
    catch (Exception e)
    {
      new ShowNotification(Notifier).Execute(new(NotificationType.Error, $"Error: {e.Message}"));
    }
  }

  [RelayCommand]
  private async Task AddNewList()
  {
    try
    {
      if (await Confirmers.ConfirmAddNewList(Confirmations.GetNewCollectionListConfirmation())
        is not (string name, string query))
        return; // Cancel

      if (string.IsNullOrEmpty(name)) throw new(Notifications.NewListNameError.Message);
      if (string.IsNullOrEmpty(query)) throw new(Notifications.NewListQueryError.Message);
      if (CollectionListViewModels.FirstOrDefault(x => x.Name == name) is not null) throw new(Notifications.NewListExistsError.Message);

      var newList = new MTGCardCollectionList() { Name = name, SearchQuery = query };

      Model.CollectionLists.Add(newList);

      if (CollectionListViewModels.LastOrDefault() is CardCollectionListViewModel listVM && listVM.Name == newList.Name)
        await ChangeList(listVM);

      new ShowNotification(Notifier).Execute(Notifications.NewListSuccess);
    }
    catch (Exception e)
    {
      new ShowNotification(Notifier).Execute(new(NotificationType.Error, $"Error: {e.Message}"));
    }
  }

  [RelayCommand]
  private async Task ChangeList(CardCollectionListViewModel? list)
  {
    ListViewModel = list;

    if (ListViewModel != null)
      await ListViewModel.RefreshCommand.ExecuteAsync(null);
  }

  private bool CanDelete() => !string.IsNullOrEmpty(CollectionName);

  private async Task OnListDelete(MTGCardCollectionList list)
  {
    if (Model.CollectionLists.Remove(list))
    {
      await ChangeList(CollectionListViewModels.FirstOrDefault());
      new ShowNotification(Notifier).Execute(Notifications.DeleteListSuccess);
    }
  }

  private void Model_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
  {
    switch (e.PropertyName)
    {
      case nameof(MTGCardCollection.Name):
        OnPropertyChanged(nameof(CollectionName));
        DeleteCollectionCommand.NotifyCanExecuteChanged();
        break;
    }

    SaveStatus.HasUnsavedChanges = true;
  }

  private void Model_CollectionLists_CollectionChanged(object? _, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
  {
    if (e.NewItems?.OfType<MTGCardCollectionList>() is var newLists && newLists != null)
    {
      foreach (var item in newLists)
        CollectionListViewModels.Add(ListViewModelFactory.Build(item));
    }
    if (e.OldItems?.OfType<MTGCardCollectionList>() is var oldLists && oldLists != null)
    {
      foreach (var item in oldLists)
      {
        if (CollectionListViewModels.TryFindIndex(x => x.Name == item.Name, out var index))
          CollectionListViewModels.RemoveAt(index);
      }
    }

    SaveStatus.HasUnsavedChanges = true;
  }
}
