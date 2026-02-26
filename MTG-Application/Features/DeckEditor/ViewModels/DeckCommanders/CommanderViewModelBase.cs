using CommunityToolkit.Mvvm.Input;
using MTGApplication.Features.DeckEditor.Models;
using MTGApplication.Features.DeckEditor.UseCases;
using MTGApplication.Features.DeckEditor.UseCases.ReversibleActions;
using MTGApplication.General.Models;
using MTGApplication.General.Services.API.CardAPI;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplication.General.Services.Importers.CardImporter.ScryfallAPI;
using MTGApplication.General.Services.IOServices;
using MTGApplication.General.Services.NotificationService.UseCases;
using MTGApplication.General.Services.ReversibleCommandService;
using MTGApplication.General.ViewModels;
using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using static MTGApplication.Features.DeckEditor.ViewModels.DeckCard.DeckCardViewModel;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplication.Features.DeckEditor.ViewModels.DeckCommanders;

public abstract partial class CommanderViewModelBase : ViewModelBase
{
  protected CommanderViewModelBase(DeckEditorMTGDeck deck)
  {
    Source = deck;

    Source.PropertyChanging += Source_PropertyChanging;
    Source.PropertyChanged += Source_PropertyChanged;
  }

  public Worker Worker { get; init; } = new();
  public ReversibleCommandStack UndoStack { get; init; } = new();
  public IMTGCardImporter Importer { private get; init; } = App.MTGCardImporter;
  public IEdhrecImporter EdhrecImporter { private get; init; } = new EdhrecImporter();
  public IScryfallImporter ScryfallImporter { private get; init; } = new ScryfallAPI();
  public Notifier Notifier { private get; init; } = new();
  public INetworkService NetworkService { private get; init; } = new NetworkService();
  public CardConfirmers Confirmers { private get; init; } = new();

  public MTGCardInfo? Info => Model?.Info;
  public string Name => Model?.Info.Name ?? string.Empty;

  protected DeckEditorMTGDeck Source { get; }
  protected abstract DeckEditorMTGCard? Model { get; set; }
  protected ReversibleFunc<DeckEditorMTGCard?> ChangeCommanderAction => field ??= new()
  {
    Action = x => Model = x,
    ReverseAction = y => Model = y
  };

  // TODO: unit test
  [RelayCommand]
  protected async Task ChangeCard(DeckEditorMTGCard? card)
  {
    try
    {
      if (card?.Info.TypeLine.Contains("Legendary") == false)
        throw new InvalidOperationException("Card type is not legendary");

      var newValue = card != null ? new DeckEditorMTGCard(card.Info) : null;

      UndoStack.PushAndExecute(
        new ReversibleValueChangeCommand<DeckEditorMTGCard?>(newValue, oldValue: Model)
        {
          ReversibleAction = ChangeCommanderAction
        });
    }
    catch (Exception e)
    {
      new ShowNotification(Notifier).Execute(new(NotificationType.Error, $"Error: {e.Message}"));
    }
  }

  // TODO: unit test
  [RelayCommand(CanExecute = nameof(CanDelete))]
  private async Task DeleteCard()
  {
    try
    {
      UndoStack.PushAndExecute(
        new ReversibleValueChangeCommand<DeckEditorMTGCard?>(null, oldValue: Model)
        {
          ReversibleAction = ChangeCommanderAction
        });
    }
    catch (Exception e)
    {
      new ShowNotification(Notifier).Execute(new(NotificationType.Error, $"Error: {e.Message}"));
    }
  }

  // TODO: unit test
  [RelayCommand]
  private async Task ImportCard(string? data)
  {
    try
    {
      ArgumentNullException.ThrowIfNull(data);

      var result = await Worker.DoWork(new ImportCards(Importer, EdhrecImporter, ScryfallImporter).Execute(data));

      if (result.Found.Length > 1) throw new InvalidOperationException("Can't set multiple commanders");
      if (result.Found.FirstOrDefault() is not CardImportResult.Card card) throw new InvalidOperationException("No cards found");
      if (!card.Info.TypeLine.Contains("Legendary")) throw new InvalidOperationException("Commander needs to be legendary");

      var editorCard = new DeckEditorMTGCard(card.Info);

      UndoStack.PushAndExecute(
        new ReversibleValueChangeCommand<DeckEditorMTGCard?>(editorCard, oldValue: Model)
        {
          ReversibleAction = new ReversibleFunc<DeckEditorMTGCard?>()
          {
            Action = x => Model = x,
            ReverseAction = y => Model = y
          }
        });

      new ShowNotification(Notifier).Execute(new(NotificationType.Success, "Commander imported successfully."));
    }
    catch (Exception e)
    {
      new ShowNotification(Notifier).Execute(new(NotificationType.Error, $"Error: {e.Message}"));
    }
  }

  // TODO: unit test
  [RelayCommand]
  protected void BeginMoveFrom(DeckEditorMTGCard? card)
  {
    ArgumentNullException.ThrowIfNull(card);

    UndoStack.ActiveCombinedCommand.Commands.Add(
      new ReversibleValueChangeCommand<DeckEditorMTGCard?>(null, oldValue: Model)
      {
        ReversibleAction = ChangeCommanderAction
      });
  }

  // TODO: unit test
  [RelayCommand]
  protected async Task BeginMoveTo(DeckEditorMTGCard? card)
  {
    ArgumentNullException.ThrowIfNull(card);

    var newValue = new DeckEditorMTGCard(card.Info);

    UndoStack.ActiveCombinedCommand.Commands.Add(
      new ReversibleValueChangeCommand<DeckEditorMTGCard?>(newValue, oldValue: Model)
      {
        ReversibleAction = ChangeCommanderAction
      });
  }

  // TODO: unit test
  [RelayCommand]
  protected void ExecuteMove() => UndoStack.PushAndExecuteActiveCombinedCommand();

  // TODO: unit test
  [RelayCommand(CanExecute = nameof(CanChangePrint))]
  private async Task ChangePrint()
  {
    try
    {
      if (Model == null)
        throw new InvalidOperationException("No card found");

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

  // TODO: unit test
  [RelayCommand(CanExecute = nameof(CanOpenCardWebsite))]
  private async Task OpenAPIWebsite()
  {
    try
    {
      if (Model == null)
        throw new InvalidOperationException("No Commander found");

      await NetworkService.OpenUri(Model.Info.APIWebsiteUri);
    }
    catch (Exception e)
    {
      new ShowNotification(Notifier).Execute(new(NotificationType.Error, $"Error: {e.Message}"));
    }
  }

  // TODO: unit test
  [RelayCommand(CanExecute = nameof(CanOpenCardWebsite))]
  private async Task OpenCardMarketWebsite()
  {
    try
    {
      if (Model == null)
        throw new InvalidOperationException("No Commander found");

      await NetworkService.OpenUri(Model.Info.CardMarketUri);
    }
    catch (Exception e)
    {
      new ShowNotification(Notifier).Execute(new(NotificationType.Error, $"Error: {e.Message}"));
    }
  }

  protected bool CanOpenCardWebsite() => Model != null;

  protected bool CanDelete() => Model != null;

  protected bool CanChangePrint() => Model != null;

  protected virtual void Source_PropertyChanging(object? sender, PropertyChangingEventArgs e) { }

  protected virtual void Source_PropertyChanged(object? sender, PropertyChangedEventArgs e)
  {
    switch (e.PropertyName)
    {
      case nameof(Source.Commander):
      case nameof(Source.CommanderPartner):
        DeleteCardCommand.NotifyCanExecuteChanged();
        OpenAPIWebsiteCommand.NotifyCanExecuteChanged();
        OpenCardMarketWebsiteCommand.NotifyCanExecuteChanged();
        ChangePrintCommand.NotifyCanExecuteChanged();
        OnPropertyChanged(nameof(Info));
        OnPropertyChanged(nameof(Name));
        break;
    }
  }

  public DeckEditorMTGCard? GetModel() => Model;
}