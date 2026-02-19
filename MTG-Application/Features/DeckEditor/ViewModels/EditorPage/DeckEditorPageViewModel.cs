using CommunityToolkit.Mvvm.Input;
using MTGApplication.Features.DeckEditor.Models;
using MTGApplication.Features.DeckEditor.UseCases;
using MTGApplication.Features.DeckEditor.ViewModels.Deck;
using MTGApplication.General.Services.Databases.Repositories;
using MTGApplication.General.Services.Databases.Repositories.DeckRepository;
using MTGApplication.General.Services.Databases.Repositories.DeckRepository.Models;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplication.General.Services.NotificationService.UseCases;
using MTGApplication.General.ViewModels;
using System;
using System.Threading.Tasks;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplication.Features.DeckEditor.ViewModels.EditorPage;

public partial class DeckEditorPageViewModel : ViewModelBase
{
  public Worker Worker { get; init; } = new();
  public IMTGCardImporter Importer { private get; init; } = App.MTGCardImporter;
  public IRepository<MTGCardDeckDTO> Repository { private get; init; } = new DeckDTORepository();
  public Notifier Notifier { private get; init; } = new();
  public EditorPageConfirmers Confirmers { private get; init; } = new();

  public string DeckName => DeckViewModel.DeckName;
  public DeckViewModel DeckViewModel
  {
    get => field ??= DeckViewModel = DeckViewModelFactory.Build(new());
    private set
    {
      var nameChanged = field?.DeckName != value?.DeckName;

      field?.PropertyChanged -= DeckViewModel_PropertyChanged;

      SetProperty(ref field, value);

      field?.PropertyChanged += DeckViewModel_PropertyChanged;

      // Visual state trigger will not work if the OnPropertyChanged(nameof(DeckName))
      //  is called when the old name was the same as the new name.
      if (nameChanged)
        OnPropertyChanged(nameof(DeckName));
    }
  }

  private DeckViewModel.Factory DeckViewModelFactory => field ??= new()
  {
    Worker = Worker,
    Repository = Repository,
    Importer = Importer,
    Notifier = Notifier,
    DeckConfirmers = Confirmers.DeckConfirmers,
    OnDeleted = OnDeckDeleted
  };

  [RelayCommand]
  private async Task NewDeck()
  {
    var saveArgs = new SaveStatus.ConfirmArgs();

    if (DeckViewModel.SaveStatus.HasUnsavedChanges)
      await DeckViewModel.SaveUnsavedChangesCommand.ExecuteAsync(saveArgs);

    if (saveArgs.Cancelled)
      return;

    await ChangeDeck(new());
  }

  [RelayCommand]
  private async Task OpenDeck(string? name)
  {
    try
    {
      var saveArgs = new SaveStatus.ConfirmArgs();

      await DeckViewModel.SaveUnsavedChangesCommand.ExecuteAsync(saveArgs);

      if (saveArgs.Cancelled)
        return; // Cancel

      var loadName = name ?? await Confirmers.ConfirmDeckOpen(Confirmations.GetOpenDeckConfirmation(
        await Worker.DoWork(new FetchDeckNames(Repository).Execute())));

      if (string.IsNullOrEmpty(loadName))
        return; // Cancel

      if (await Worker.DoWork(new FetchDeck(Repository, Importer).Execute(loadName)) is DeckEditorMTGDeck deck)
      {
        await ChangeDeck(deck);

        new ShowNotification(Notifier).Execute(Notifications.LoadSuccess);
      }
      else
        new ShowNotification(Notifier).Execute(Notifications.LoadError);
    }
    catch (Exception e)
    {
      new ShowNotification(Notifier).Execute(new(NotificationType.Error, e.Message));
    }
  }

  private async Task ChangeDeck(DeckEditorMTGDeck? deck)
  {
    ArgumentNullException.ThrowIfNull(deck);

    DeckViewModel = DeckViewModelFactory.Build(deck);
  }

  private async Task OnDeckDeleted() => await ChangeDeck(new());

  private void DeckViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
  {
    switch (e.PropertyName)
    {
      case nameof(DeckViewModel.DeckName): OnPropertyChanged(nameof(DeckName)); break;
    }
  }
}