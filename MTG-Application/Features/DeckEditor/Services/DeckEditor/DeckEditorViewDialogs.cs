using MTGApplication.General.Models.Card;
using MTGApplication.General.Services.ConfirmationService.Extensions;
using MTGApplication.General.Views.Dialogs;
using System;
using System.Linq;
using static MTGApplication.General.Services.ConfirmationService.DialogService;

namespace MTGApplication.Features.DeckEditor.Services.DeckEditor;

public class DeckEditorViewDialogs : IViewDialogs<DeckEditorConfirmers>
{
  public static void RegisterConfirmDialogs(DeckEditorConfirmers confirmers, Func<DialogWrapper> getWrapper)
  {
    confirmers.SaveUnsavedChangesConfirmer.OnConfirm = async msg => await new ShowUnsavedChangesDialog(getWrapper.Invoke()).Execute((msg.Title, msg.Message));
    confirmers.LoadDeckConfirmer.OnConfirm = async msg => await new ShowOpenDialog(getWrapper.Invoke()).Execute((msg.Title, msg.Message, msg.Data));
    confirmers.SaveDeckConfirmer.OnConfirm = async msg => await new ShowSaveDialog(getWrapper.Invoke()).Execute((msg.Title, msg.Message, msg.Data));
    confirmers.OverrideDeckConfirmer.OnConfirm = async msg => await new ShowOverrideDialog(getWrapper.Invoke()).Execute((msg.Title, msg.Message));
    confirmers.DeleteDeckConfirmer.OnConfirm = async msg => await new ShowDeleteDialog(getWrapper.Invoke()).Execute((msg.Title, msg.Message));
    confirmers.ShowTokensConfirmer.OnConfirm = async (msg)
      => await new GridViewDialog<DeckEditorMTGCard>(msg.Title, "MTGPrintGridViewItemTemplate", "MTGAdaptiveGridViewStyle")
      {
        Items = msg.Data.ToArray(),
        PrimaryButtonText = string.Empty,
        SecondaryButtonText = string.Empty,
        CloseButtonText = "Close"
      }.ShowAsync(getWrapper.Invoke()) as DeckEditorMTGCard;

    confirmers.CardListConfirmers.ExportConfirmer.OnConfirm = async msg => await new TextAreaDialog(msg.Title)
    {
      TextInputText = msg.Data,
      PrimaryButtonText = "Copy to Clipboard",
      SecondaryButtonText = string.Empty
    }.ShowAsync(getWrapper.Invoke());
    confirmers.CardListConfirmers.ImportConfirmer.OnConfirm = async msg => await new TextAreaDialog(msg.Title)
    {
      TextInputText = msg.Data,
      InputPlaceholderText = "Example:\n2 Black Lotus\nMox Ruby\nbd8fa327-dd41-4737-8f19-2cf5eb1f7cdd",
      PrimaryButtonText = "Import",
      SecondaryButtonText = string.Empty
    }.ShowAsync(getWrapper.Invoke());
    confirmers.CardListConfirmers.AddMultipleConflictConfirmer.OnConfirm = async (msg) =>
    {
      var (answer, isChecked) = await new CheckBoxDialog(msg.Title)
      {
        Message = msg.Message,
        InputText = "Same for all cards.",
        SecondaryButtonText = string.Empty,
        CloseButtonText = "No"
      }.ShowAsync(getWrapper.Invoke());

      return new(answer.ToConfirmationResult(), isChecked ?? false);
    };
    confirmers.CardListConfirmers.AddSingleConflictConfirmer.OnConfirm = async (msg) => (await new ConfirmationDialog(msg.Title)
    {
      Message = msg.Message,
      SecondaryButtonText = string.Empty,
      CloseButtonText = "No",
      PrimaryButtonText = "Yes"
    }.ShowAsync(getWrapper.Invoke())).ToConfirmationResult();
    confirmers.CardListConfirmers.ChangeCardPrintConfirmer.OnConfirm = async (msg)
      => await new GridViewDialog<DeckEditorMTGCard>(msg.Title, "MTGPrintGridViewItemTemplate", "MTGAdaptiveGridViewStyle")
      {
        Items = msg.Data.ToArray(),
        SecondaryButtonText = string.Empty
      }.ShowAsync(getWrapper.Invoke()) as DeckEditorMTGCard;
    confirmers.CommanderConfirmers.ChangeCardPrintConfirmer.OnConfirm = async (msg)
      => await new GridViewDialog<DeckEditorMTGCard>(msg.Title, "MTGPrintGridViewItemTemplate", "MTGAdaptiveGridViewStyle")
      {
        Items = msg.Data.ToArray(),
        PrimaryButtonText = "Change",
        SecondaryButtonText = string.Empty,
      }.ShowAsync(getWrapper.Invoke()) as DeckEditorMTGCard;
  }
}