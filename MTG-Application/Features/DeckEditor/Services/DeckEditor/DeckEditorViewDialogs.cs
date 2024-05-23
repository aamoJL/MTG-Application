using MTGApplication.General.Models.Card;
using MTGApplication.General.Services.ConfirmationService;
using MTGApplication.General.Views.Dialogs;
using System;
using System.Linq;
using static MTGApplication.General.Services.ConfirmationService.DialogService;

namespace MTGApplication.Features.DeckEditor;

public class DeckEditorViewDialogs
{
  public static void RegisterConfirmDialogs(DeckEditorConfirmers confirmer, Func<DialogWrapper> getWrapper)
  {
    confirmer.SaveUnsavedChangesConfirmer.OnConfirm = async msg => await new ShowUnsavedChangesDialog(getWrapper.Invoke()).Execute((msg.Title, msg.Message));
    confirmer.LoadDeckConfirmer.OnConfirm = async msg => await new ShowOpenDialog(getWrapper.Invoke()).Execute((msg.Title, msg.Message, msg.Data));
    confirmer.SaveDeckConfirmer.OnConfirm = async msg => await new ShowSaveDialog(getWrapper.Invoke()).Execute((msg.Title, msg.Message, msg.Data));
    confirmer.OverrideDeckConfirmer.OnConfirm = async msg => await new ShowOverrideDialog(getWrapper.Invoke()).Execute((msg.Title, msg.Message));
    confirmer.DeleteDeckConfirmer.OnConfirm = async msg => await new ShowDeleteDialog(getWrapper.Invoke()).Execute((msg.Title, msg.Message));

    confirmer.CardListConfirmers.ExportConfirmer.OnConfirm = async msg => await new TextAreaDialog(msg.Title)
    {
      TextInputText = msg.Data,
      PrimaryButtonText = "Copy to Clipboard",
      SecondaryButtonText = string.Empty
    }.ShowAsync(getWrapper.Invoke());
    confirmer.CardListConfirmers.ImportConfirmer.OnConfirm = async msg => await new TextAreaDialog(msg.Title)
    {
      TextInputText = msg.Data,
      InputPlaceholderText = "Example:\n2 Black Lotus\nMox Ruby\nbd8fa327-dd41-4737-8f19-2cf5eb1f7cdd",
      PrimaryButtonText = "Import",
      SecondaryButtonText = string.Empty
    }.ShowAsync(getWrapper.Invoke());
    confirmer.CardListConfirmers.AddMultipleConflictConfirmer.OnConfirm = async (msg) =>
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
    confirmer.CardListConfirmers.AddSingleConflictConfirmer.OnConfirm = async (msg) => (await new ConfirmationDialog(msg.Title)
    {
      Message = msg.Message,
      SecondaryButtonText = string.Empty,
      CloseButtonText = "No",
      PrimaryButtonText = "Yes"
    }.ShowAsync(getWrapper.Invoke())).ToConfirmationResult();
    confirmer.CardListConfirmers.ChangeCardPrintConfirmer.OnConfirm = async (msg)
      => (await new GridViewDialog<MTGCard>(msg.Title, "MTGPrintGridViewItemTemplate", "MTGAdaptiveGridViewStyle")
      {
        Items = msg.Data.ToArray(),
        SecondaryButtonText = string.Empty
      }.ShowAsync(getWrapper.Invoke())) as MTGCard;

    confirmer.CommanderConfirmers.ChangeCardPrintConfirmer.OnConfirm = async (msg)
      => (await new GridViewDialog<MTGCard>(msg.Title, "MTGPrintGridViewItemTemplate", "MTGAdaptiveGridViewStyle")
      {
        Items = msg.Data.ToArray(),
        PrimaryButtonText = "Change",
        SecondaryButtonText = string.Empty,
      }.ShowAsync(getWrapper.Invoke())) as MTGCard;
  }
}