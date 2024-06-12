using MTGApplication.Features.CardCollection.Services;
using MTGApplication.General.Models;
using MTGApplication.General.Views.Dialogs;
using MTGApplication.General.Views.Dialogs.UseCases;
using System;
using System.Linq;
using static MTGApplication.General.Services.ConfirmationService.DialogService;

namespace MTGApplication.Features.CardCollection;

public partial class CardCollectionViewDialogs : IViewDialogs<CardCollectionConfirmers>
{
  public static void RegisterConfirmDialogs(CardCollectionConfirmers confirmers, Func<DialogWrapper> getWrapper)
  {
    confirmers.SaveUnsavedChangesConfirmer.OnConfirm = async msg => await new ShowUnsavedChangesDialog(getWrapper.Invoke()).Execute((msg.Title, msg.Message));
    confirmers.LoadCollectionConfirmer.OnConfirm = async msg => await new ShowOpenDialog(getWrapper.Invoke()).Execute((msg.Title, msg.Message, msg.Data.ToArray()));
    confirmers.SaveCollectionConfirmer.OnConfirm = async msg => await new ShowSaveDialog(getWrapper.Invoke()).Execute((msg.Title, msg.Message, msg.Data));
    confirmers.OverrideCollectionConfirmer.OnConfirm = async msg => await new ShowOverrideDialog(getWrapper.Invoke()).Execute((msg.Title, msg.Message));
    confirmers.DeleteCollectionConfirmer.OnConfirm = async msg => await new ShowDeleteDialog(getWrapper.Invoke()).Execute((msg.Title, msg.Message));

    confirmers.NewCollectionListConfirmer.OnConfirm = async msg => await new ShowCollectionListContentDialog(getWrapper.Invoke()).Execute((msg.Title, msg.Message, null));
    confirmers.EditCollectionListConfirmer.OnConfirm = async msg => await new ShowCollectionListContentDialog(getWrapper.Invoke()).Execute((msg.Title, msg.Message, msg.Data));
    confirmers.DeleteCollectionListConfirmer.OnConfirm = async msg => await new ShowDeleteDialog(getWrapper.Invoke()).Execute((msg.Title, msg.Message));
    confirmers.ImportCardsConfirmer.OnConfirm = async msg => await new TextAreaDialog(msg.Title)
    {
      InputPlaceholderText = "Example:\ned0216a0-c5c9-4a99-b869-53e4d0256326\n45fd6e91-df76-497f-b642-33dc3d5f6a5a\nbd8fa327-dd41-4737-8f19-2cf5eb1f7cdd",
      PrimaryButtonText = "Import",
      SecondaryButtonText = string.Empty
    }.ShowAsync(getWrapper.Invoke());
    confirmers.ExportCardsConfirmer.OnConfirm = async msg => await new TextAreaDialog(msg.Title)
    {
      TextInputText = msg.Data,
      PrimaryButtonText = "Copy to Clipboard",
      SecondaryButtonText = string.Empty
    }.ShowAsync(getWrapper.Invoke());
    confirmers.ShowCardPrintsConfirmer.OnConfirm = async (msg)
      => (await new GridViewDialog<MTGCardInfo>(msg.Title, "MTGPrintGridViewItemTemplate", "MTGAdaptiveGridViewStyle")
      {
        Items = msg.Data.ToArray(),
        SecondaryButtonText = string.Empty
      }.ShowAsync(getWrapper.Invoke())) as MTGCardInfo;
  }
}