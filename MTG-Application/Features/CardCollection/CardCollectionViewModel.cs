using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MTGApplication.General.Databases.Repositories;
using MTGApplication.General.Models.Card;
using MTGApplication.General.Models.CardCollection;
using MTGApplication.General.Services.API.CardAPI;
using MTGApplication.General.Services.Databases.Repositories.CardCollectionRepository;
using MTGApplication.General.Services.Databases.Repositories.CardCollectionRepository.UseCases;
using MTGApplication.General.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace MTGApplication.Features.CardCollection;

public partial class CardCollectionViewModel(ICardAPI<MTGCard> cardAPI) : ViewModelBase, IWorker, ISavable
{
  [ObservableProperty] private MTGCardCollection collection = new();
  [ObservableProperty,
    NotifyPropertyChangedFor(nameof(SelectedListCardCount))]
  private MTGCardCollectionList selectedList;
  [ObservableProperty] private bool isBusy;
  [ObservableProperty] private bool hasUnsavedChanges;

  public IncrementalLoadingCardCollection<CardCollectionMTGCard> QueryCards { get; } = new(new CardCollectionIncrementalCardSource(cardAPI));
  public CardCollectionConfirmers Confirmers { get; init; }
  public IRepository<MTGCardCollectionDTO> Repository { get; init; } = new CardCollectionDTORepository();

  public int SelectedListCardCount => SelectedList?.Cards.Count ?? 0;

  private ICardAPI<MTGCard> CardAPI { get; } = cardAPI;

  [RelayCommand]
  private async Task NewCollection()
  {
    if (!await ConfirmUnsavedChanges())
      return;

    Collection = new();
    HasUnsavedChanges = false;
  }

  [RelayCommand]
  private async Task OpenCollection()
  {
    if (!await ConfirmUnsavedChanges())
      return;

    if (await ((IWorker)this).DoWork(Confirmers.LoadCollectionConfirmer.Confirm(
      CardCollectionConfirmers.GetLoadCollectionConfirmation(
        await new GetCardCollectionNames(Repository).Execute()))) is not string loadName)
      return;

    if (await ((IWorker)this).DoWork(new GetCardCollection(Repository, CardAPI).Execute(loadName)) is MTGCardCollection loadedCollection)
    {
      Collection = loadedCollection;
      SelectedList = Collection.CollectionLists.FirstOrDefault();
      // TODO: RaiseInAppNotification(NotificationService.NotificationType.Success, "The collection was loaded successfully.");
    }
    else
    {
      // TODO: RaiseInAppNotification(NotificationService.NotificationType.Error, "Error. Could not load the collection.")
    }
  }

  [RelayCommand]
  private async Task SaveCollection()
  {
    // TODO:
  }

  [RelayCommand]
  private async Task DeleteCollection()
  {
    // TODO:
  }

  [RelayCommand]
  private async Task NewList()
  {
    // TODO:
  }

  [RelayCommand]
  private async Task EditList()
  {
    // TODO:
  }

  [RelayCommand]
  private async Task ImportCards()
  {
    // TODO:
  }

  [RelayCommand]
  private async Task ExportCards()
  {
    // TODO:
  }

  [RelayCommand]
  private async Task DeleteList()
  {
    // TODO:
  }

  public Task<bool> ConfirmUnsavedChanges()
  {
    // TODO:
    return Task.FromResult(HasUnsavedChanges);
  }
}