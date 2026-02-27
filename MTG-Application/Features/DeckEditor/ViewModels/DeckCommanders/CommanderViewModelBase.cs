using CommunityToolkit.Mvvm.Input;
using MTGApplication.Features.DeckEditor.Models;
using MTGApplication.Features.DeckEditor.UseCases;
using MTGApplication.Features.DeckEditor.UseCases.ReversibleActions;
using MTGApplication.General.Models;
using MTGApplication.General.Services.Importers.CardImporter;
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

  public required DeckEditorDependencies EditorDependencies { get; init; }
  public required ReversibleCommandStack UndoStack { get; init; } = new();

  public MTGCardInfo? Info => Model?.Info;
  public string Name => Model?.Info.Name ?? string.Empty;

  protected DeckEditorMTGDeck Source { get; }
  protected abstract DeckEditorMTGCard? Model { get; set; }
  protected ReversibleFunc<DeckEditorMTGCard?> ChangeCommanderAction => field ??= new()
  {
    Action = x => Model = x,
    ReverseAction = y => Model = y
  };

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
      new ShowNotification(EditorDependencies.Notifier).Execute(new(NotificationType.Error, $"Error: {e.Message}"));
    }
  }

  [RelayCommand(CanExecute = nameof(CanDelete))]
  private void DeleteCard()
  {
    UndoStack.PushAndExecute(
      new ReversibleValueChangeCommand<DeckEditorMTGCard?>(null, oldValue: Model)
      {
        ReversibleAction = ChangeCommanderAction
      });
  }

  [RelayCommand]
  private async Task ImportCard(string? data)
  {
    try
    {
      ArgumentNullException.ThrowIfNull(data);

      var result = await EditorDependencies.Worker.DoWork(new ImportCards(EditorDependencies.Importer, EditorDependencies.EdhrecImporter, EditorDependencies.ScryfallImporter).Execute(data));

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

      new ShowNotification(EditorDependencies.Notifier).Execute(new(NotificationType.Success, "Commander imported successfully."));
    }
    catch (Exception e)
    {
      new ShowNotification(EditorDependencies.Notifier).Execute(new(NotificationType.Error, $"Error: {e.Message}"));
    }
  }

  [RelayCommand]
  protected void BeginMoveFrom(DeckEditorMTGCard? card)
  {
    ArgumentNullException.ThrowIfNull(card);

    if (card != Model)
      throw new InvalidOperationException("Card is not the commander");

    UndoStack.ActiveCombinedCommand.Commands.Add(
      new ReversibleValueChangeCommand<DeckEditorMTGCard?>(null, oldValue: Model)
      {
        ReversibleAction = ChangeCommanderAction
      });
  }

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

  [RelayCommand]
  protected void ExecuteMove() => UndoStack.PushAndExecuteActiveCombinedCommand();

  [RelayCommand(CanExecute = nameof(CanChangePrint))]
  private async Task ChangePrint()
  {
    try
    {
      if (Model == null)
        throw new InvalidOperationException("Commander is not set");

      var prints = (await EditorDependencies.Worker.DoWork(new FetchCardPrints(EditorDependencies.Importer).Execute(Model))).Found.Select(x => new MTGCard(x.Info));

      if (await EditorDependencies.CardConfirmers.ConfirmCardPrints(Confirmations.GetChangeCardPrintConfirmation(prints)) is not MTGCard selection)
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
      new ShowNotification(EditorDependencies.Notifier).Execute(new(NotificationType.Error, $"Error: {e.Message}"));
    }
  }

  [RelayCommand(CanExecute = nameof(CanOpenCardWebsite))]
  private async Task OpenAPIWebsite()
  {
    try
    {
      if (Model == null)
        throw new InvalidOperationException("No Commander found");

      await EditorDependencies.NetworkService.OpenUri(Model.Info.APIWebsiteUri);
    }
    catch (Exception e)
    {
      new ShowNotification(EditorDependencies.Notifier).Execute(new(NotificationType.Error, $"Error: {e.Message}"));
    }
  }

  [RelayCommand(CanExecute = nameof(CanOpenCardWebsite))]
  private async Task OpenCardMarketWebsite()
  {
    try
    {
      if (Model == null)
        throw new InvalidOperationException("No Commander found");

      await EditorDependencies.NetworkService.OpenUri(Model.Info.CardMarketUri);
    }
    catch (Exception e)
    {
      new ShowNotification(EditorDependencies.Notifier).Execute(new(NotificationType.Error, $"Error: {e.Message}"));
    }
  }

  public DeckEditorMTGCard? GetModel() => Model;

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
}