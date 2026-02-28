using CommunityToolkit.Mvvm.Input;
using MTGApplication.Features.DeckEditor.Models;
using MTGApplication.General.Services.NotificationService.UseCases;
using MTGApplication.General.Services.ReversibleCommandService;
using MTGApplication.General.ViewModels;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplication.Features.DeckEditor.ViewModels.DeckCommanders;

public partial class DeckCommandersViewModel : ViewModelBase
{
  public DeckCommandersViewModel(DeckEditorMTGDeck deck)
  {
    Model = deck;
    Model.PropertyChanged += Model_PropertyChanged;
  }

  public required DeckEditorDependencies EditorDependencies { get; init; }
  public required ReversibleCommandStack UndoStack { get; init; }

  public CommanderViewModel Commander { get => field ??= CreateCommanderViewModel(Model); private set; }
  public PartnerViewModel Partner { get => field ??= CreatePartnerViewModel(Model); private set; }

  private DeckEditorMTGDeck Model { get; }

  [RelayCommand(CanExecute = nameof(CanOpenEdhrecWebsite))]
  private async Task OpenEdhrecWebsite()
  {
    try
    {
      if (Model.Commander == null)
        throw new InvalidOperationException("Deck does not have a commander");

      await EditorDependencies.NetworkService.OpenUri(
        General.Services.Importers.CardImporter.EdhrecImporter.GetCommanderWebsiteUri(Model.Commander!, Model.CommanderPartner));
    }
    catch (Exception e)
    {
      new ShowNotification(EditorDependencies.Notifier).Execute(new(NotificationType.Error, $"Error: {e.Message}"));
    }
  }

  private bool CanOpenEdhrecWebsite() => Model.Commander != null;

  private void Model_PropertyChanged(object? sender, PropertyChangedEventArgs e)
  {
    switch (e.PropertyName)
    {
      case nameof(Model.Commander): OpenEdhrecWebsiteCommand.NotifyCanExecuteChanged(); break;
    }
  }

  private CommanderViewModel CreateCommanderViewModel(DeckEditorMTGDeck deck) => new(deck)
  {
    EditorDependencies = EditorDependencies,
    UndoStack = UndoStack,
  };

  private PartnerViewModel CreatePartnerViewModel(DeckEditorMTGDeck deck) => new(deck)
  {
    EditorDependencies = EditorDependencies,
    UndoStack = UndoStack,
  };
}