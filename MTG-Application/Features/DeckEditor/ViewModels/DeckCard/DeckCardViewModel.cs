using CommunityToolkit.Mvvm.Input;
using MTGApplication.Features.DeckEditor.Models;
using MTGApplication.Features.DeckEditor.UseCases;
using MTGApplication.Features.DeckEditor.UseCases.ReversibleActions;
using MTGApplication.General.Models;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplication.General.Services.IOServices;
using MTGApplication.General.Services.NotificationService.UseCases;
using MTGApplication.General.Services.ReversibleCommandService;
using MTGApplication.General.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplication.Features.DeckEditor.ViewModels.DeckCard;

public partial class DeckCardViewModel : ViewModelBase
{
  public DeckCardViewModel(DeckEditorMTGCard card)
  {
    Model = card;
    Model.PropertyChanged += Model_PropertyChanged;
  }

  public Worker Worker { get; init; } = new();
  public IMTGCardImporter Importer { private get; init; } = App.MTGCardImporter;
  public ReversibleCommandStack UndoStack { private get; init; } = new();
  public Notifier Notifier { private get; init; } = new();
  public INetworkService NetworkService { private get; init; } = new NetworkService();
  public CardConfirmers Confirmers { private get; init; } = new();

  public string Name => Model.Info.Name;
  public MTGCardInfo Info => Model.Info;
  public int Count => Model.Count;
  public string Group => Model.Group;
  public CardTag? CardTag => Model.CardTag;

  public Action<DeckEditorMTGCard>? OnDelete { get => field ?? throw new NotImplementedException(); init; }

  private DeckEditorMTGCard Model { get; }

  [RelayCommand]
  private void ChangeCount(int? value)
  {
    if (value == null)
      return;

    UndoStack.PushAndExecute(
      new ReversiblePropertyChangeCommand<DeckEditorMTGCard, int>(Model, Model.Count, (int)value)
      {
        ReversibleAction = new ReversibleCardCountChangeAction()
      });
  }

  [RelayCommand]
  private async Task ChangePrint()
  {
    try
    {
      var prints = (await Worker.DoWork(new FetchCardPrints(Importer).Execute(Model))).Found.Select(x => new MTGCard(x.Info));

      if (await Confirmers.ConfirmCardPrints(Confirmations.GetChangeCardPrintConfirmation(prints)) is not MTGCard selection)
        return; // Cancel

      if (selection.Info.ScryfallId == Model.Info.ScryfallId)
        return; // Cancel, Same print

      UndoStack.PushAndExecute(
        new ReversiblePropertyChangeCommand<DeckEditorMTGCard, MTGCardInfo>(Model, Model.Info, selection.Info)
        {
          ReversibleAction = new ReversibleCardPrintChangeAction()
        });
    }
    catch (Exception e)
    {
      new ShowNotification(Notifier).Execute(new(NotificationType.Error, $"Error: {e.Message}"));
    }
  }

  [RelayCommand]
  private void ChangeTag(CardTag? value)
  {
    UndoStack.PushAndExecute(
      new ReversiblePropertyChangeCommand<DeckEditorMTGCard, CardTag?>(Model, Model.CardTag, value)
      {
        ReversibleAction = new ReversibleCardTagChangeAction()
      });
  }

  [RelayCommand]
  private void DeleteCard()
  {
    try
    {
      OnDelete?.Invoke(Model);
    }
    catch (Exception e)
    {
      new ShowNotification(Notifier).Execute(new(NotificationType.Error, $"Error: {e.Message}"));
    }
  }

  [RelayCommand]
  private async Task OpenAPIWebsite()
  {
    try
    {
      await NetworkService.OpenUri(Model.Info.APIWebsiteUri);
    }
    catch (Exception e)
    {
      new ShowNotification(Notifier).Execute(new(NotificationType.Error, $"Error: {e.Message}"));
    }
  }

  [RelayCommand]
  private async Task OpenCardMarketWebsite()
  {
    try
    {
      await NetworkService.OpenUri(Model.Info.CardMarketUri);
    }
    catch (Exception e)
    {
      new ShowNotification(Notifier).Execute(new(NotificationType.Error, $"Error: {e.Message}"));
    }
  }

  public DeckEditorMTGCard CopyModel() => Model.Copy();

  private void Model_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
  {
    switch (e.PropertyName)
    {
      case nameof(Model.Info): OnPropertyChanged(nameof(Info)); break;
      case nameof(Model.Count): OnPropertyChanged(nameof(Count)); break;
      case nameof(Model.Group): OnPropertyChanged(nameof(Group)); break;
      case nameof(Model.CardTag): OnPropertyChanged(nameof(CardTag)); break;
    }
  }
}
