using CommunityToolkit.Mvvm.Input;
using MTGApplication.Features.DeckEditor.Models;
using MTGApplication.Features.DeckEditor.UseCases;
using MTGApplication.Features.DeckEditor.ViewModels.Deck;
using MTGApplication.General.Services.NotificationService.UseCases;
using MTGApplication.General.ViewModels;
using System;
using System.Threading.Tasks;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplication.Features.DeckEditor.ViewModels.EditorPage;

public partial class DeckEditorPageViewModel : ViewModelBase
{
  public DeckEditorDependencies EditorDependencies { get; init; } = new();

  public string DeckName => DeckViewModel.DeckName;
  public DeckViewModel DeckViewModel
  {
    get => field ??= DeckViewModel = CreateDeckViewModel(new());
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

      OnPropertyChanged(nameof(SaveStatus));
    }
  }

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

      var loadName = name ?? await EditorDependencies.PageConfirmers.ConfirmDeckOpen(Confirmations.GetOpenDeckConfirmation(
        await EditorDependencies.Worker.DoWork(new FetchDeckNames(EditorDependencies.Repository).Execute())));

      if (string.IsNullOrEmpty(loadName))
        return; // Cancel

      if (await EditorDependencies.Worker.DoWork(new FetchDeck(EditorDependencies.Repository, EditorDependencies.Importer).Execute(loadName)) is DeckEditorMTGDeck deck)
      {
        await ChangeDeck(deck);

        new ShowNotification(EditorDependencies.Notifier).Execute(Notifications.LoadSuccess);
      }
      else
        new ShowNotification(EditorDependencies.Notifier).Execute(Notifications.LoadError);
    }
    catch (Exception e)
    {
      new ShowNotification(EditorDependencies.Notifier).Execute(new(NotificationType.Error, e.Message));
    }
  }

  private async Task ChangeDeck(DeckEditorMTGDeck? deck)
  {
    ArgumentNullException.ThrowIfNull(deck);

    DeckViewModel = CreateDeckViewModel(deck);
  }

  private async Task OnDeckDeleted() => await ChangeDeck(new());

  private void DeckViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
  {
    switch (e.PropertyName)
    {
      case nameof(DeckViewModel.DeckName): OnPropertyChanged(nameof(DeckName)); break;
    }
  }

  private DeckViewModel CreateDeckViewModel(DeckEditorMTGDeck deck) => new(deck)
  {
    EditorDependencies = EditorDependencies,
    OnDeleted = OnDeckDeleted,
  };
}