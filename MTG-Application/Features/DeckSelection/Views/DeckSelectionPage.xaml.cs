using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MTGApplication.Features.DeckSelection.Models;
using MTGApplication.Features.DeckSelection.ViewModels.SelectionPage;
using System;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplication.Features.DeckSelection.Views;

public sealed partial class DeckSelectionPage : Page
{
  public DeckSelectionPage()
  {
    InitializeComponent();

    Loaded += MTGDeckSelectorView_Loaded;
  }

  public DeckSelectionPageViewModel ViewModel => field ??= new()
  {
    Notifier = Notifier,
    OnSelected = deck => OnDeckSelected?.Invoke(deck)
  };

  private Notifier Notifier
  {
    get => field ??= Notifier = new();
    set
    {
      if (field == value) return;
      field?.OnNotifyEvent -= Notifier_OnNotifyEvent;
      field = value;
      field?.OnNotifyEvent += Notifier_OnNotifyEvent;
    }
  }

  public Action<DeckSelectionDeck>? OnDeckSelected { private get; set; } = null;

  private void MTGDeckSelectorView_Loaded(object sender, RoutedEventArgs e)
  {
    Loaded -= MTGDeckSelectorView_Loaded;

    if (ViewModel.RefreshDecksCommand.CanExecute(null))
      _ = ViewModel.RefreshDecksCommand.ExecuteAsync(null);
  }

  private void Notifier_OnNotifyEvent(object? _, Notification e)
    => RaiseNotification(this, e);
}
