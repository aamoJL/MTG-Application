using Microsoft.UI.Xaml;
using MTGApplication.Features.CardCollectionEditor.CardCollection.Services;
using MTGApplication.Features.CardCollectionEditor.CardCollectionList.Services;
using MTGApplication.Features.CardCollectionEditor.Editor.Services;
using MTGApplication.General.Models;
using MTGApplication.General.Services.ConfirmationService;
using MTGApplication.General.Views.Dialogs;
using MTGApplication.General.Views.Dialogs.Controls;
using MTGApplication.General.Views.Dialogs.UseCases;
using System.Linq;
using static MTGApplication.Features.CardCollectionEditor.CardCollectionList.Views.CardCollectionEditorViewDialogs;

namespace MTGApplication.Features.CardCollection.Editor.Services;

public partial class CardCollectionEditorViewDialogs : IViewDialogs<CardCollectionEditorConfirmers>
{
  public static void RegisterConfirmDialogs(CardCollectionEditorConfirmers confirmers, XamlRoot root)
  {
    confirmers.SaveUnsavedChangesConfirmer.OnConfirm = async msg => await new ShowUnsavedChangesDialog(root).Execute((msg.Title, msg.Message));
    confirmers.LoadCollectionConfirmer.OnConfirm = async msg => await new ShowOpenDialog(root).Execute((msg.Title, msg.Message, msg.Data.ToArray()));

    RegisterCardCollectionDialogs(confirmers.CardCollectionConfirmers, root);
    RegisterCardCollectionListDialogs(confirmers.CardCollectionListConfirmers, root);
  }

  private static void RegisterCardCollectionDialogs(CardCollectionConfirmers confirmers, XamlRoot root)
  {
    confirmers.SaveCollectionConfirmer.OnConfirm = async msg => await new ShowSaveDialog(root).Execute((msg.Title, msg.Message, msg.Data));
    confirmers.OverrideCollectionConfirmer.OnConfirm = async msg => await new ShowOverrideDialog(root).Execute((msg.Title, msg.Message));
    confirmers.DeleteCollectionConfirmer.OnConfirm = async msg => await new ShowDeleteDialog(root).Execute((msg.Title, msg.Message));
    confirmers.NewCollectionListConfirmer.OnConfirm = async msg => await new ShowCollectionListContentDialog(root).Execute((msg.Title, msg.Message, null));
    confirmers.DeleteCollectionListConfirmer.OnConfirm = async msg => await new ShowDeleteDialog(root).Execute((msg.Title, msg.Message));
  }

  private static void RegisterCardCollectionListDialogs(CardCollectionListConfirmers confirmers, XamlRoot root)
  {
    confirmers.EditCollectionListConfirmer.OnConfirm = async msg => await new ShowCollectionListContentDialog(root).Execute((msg.Title, msg.Message, msg.Data));
    confirmers.EditCollectionListQueryConflictConfirmer.OnConfirm = async msg => await DialogService.ShowAsync(root, new TwoButtonConfirmationDialog(msg.Title, msg.Message)
    {
      PrimaryButtonText = "OK"
    });
    confirmers.ImportCardsConfirmer.OnConfirm = async msg => await DialogService.ShowAsync(root, new TextAreaDialog(msg.Title)
    {
      InputPlaceholderText = "Example:\ned0216a0-c5c9-4a99-b869-53e4d0256326\n45fd6e91-df76-497f-b642-33dc3d5f6a5a\nbd8fa327-dd41-4737-8f19-2cf5eb1f7cdd",
      PrimaryButtonText = "Import",
    });
    confirmers.ExportCardsConfirmer.OnConfirm = async msg => await DialogService.ShowAsync(root, new TextAreaDialog(msg.Title)
    {
      InputText = msg.Data,
      PrimaryButtonText = "Copy to Clipboard",
    });
    confirmers.ShowCardPrintsConfirmer.OnConfirm = async (msg) =>
    {
      Application.Current.Resources.TryGetValue("MTGPrintGridViewItemTemplate", out var template);

      return (await DialogService.ShowAsync(root, new GridViewDialog(
        title: msg.Title,
        items: msg.Data.ToArray(),
        itemTemplate: (DataTemplate)template))) as MTGCard;
    };
  }
}