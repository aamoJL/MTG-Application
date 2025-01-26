using Microsoft.UI.Xaml;
using MTGApplication.General.Models;
using MTGApplication.General.Services.ConfirmationService;
using MTGApplication.General.Views.Dialogs;
using MTGApplication.General.Views.Dialogs.Controls;
using MTGApplication.General.Views.Dialogs.UseCases;
using System.IO;
using System.Linq;

namespace MTGApplication.Features.DeckEditor.Editor.Services;

public class DeckEditorViewDialogs : IViewDialogs<DeckEditorConfirmers>
{
  public static void RegisterConfirmDialogs(DeckEditorConfirmers confirmers, XamlRoot root)
  {
    confirmers.SaveUnsavedChangesConfirmer.OnConfirm = async msg => await new ShowUnsavedChangesDialog(root).Execute((msg.Title, msg.Message));
    confirmers.LoadDeckConfirmer.OnConfirm = async msg => await new ShowOpenDialog(root).Execute((msg.Title, msg.Message, msg.Data));
    confirmers.SaveDeckConfirmer.OnConfirm = async msg => await new ShowSaveDialog(root).Execute((msg.Title, msg.Message, msg.Data));
    confirmers.OverrideDeckConfirmer.OnConfirm = async msg => await new ShowOverrideDialog(root).Execute((msg.Title, msg.Message));
    confirmers.DeleteDeckConfirmer.OnConfirm = async msg => await new ShowDeleteDialog(root).Execute((msg.Title, msg.Message));
    confirmers.ShowTokensConfirmer.OnConfirm = async (msg) =>
    {
      Application.Current.Resources.TryGetValue("MTGPrintGridViewItemTemplate", out var template);

      return (await DialogService.ShowAsync(root, new GridViewDialog(
        title: msg.Title,
        items: msg.Data.ToArray(),
        itemTemplate: (DataTemplate)template)
      {
        PrimaryButtonText = string.Empty,
        CloseButtonText = "Close",
        CanSelectItems = false,
      })) as MTGCard;
    };

    confirmers.CardListConfirmers.ExportConfirmer.OnConfirm = async msg
      => await DialogService.ShowAsync(root, new TextAreaDialog(msg.Title)
      {
        InputText = msg.Data,
        PrimaryButtonText = "Copy to Clipboard",
      });
    confirmers.CardListConfirmers.ImportConfirmer.OnConfirm = async msg
      => await DialogService.ShowAsync(root, new TextAreaDialog(msg.Title)
      {
        InputText = msg.Data,
        InputPlaceholderText = "Example:\n2 Black Lotus\nMox Ruby\nbd8fa327-dd41-4737-8f19-2cf5eb1f7cdd",
        PrimaryButtonText = "Import",
      });
    confirmers.CardListConfirmers.AddMultipleConflictConfirmer.OnConfirm = async (msg)
      => await DialogService.ShowAsync(root, new CheckBoxDialog(msg.Title, msg.Message)
      {
        InputText = "Same for all cards.",
        CloseButtonText = "No",
      });
    confirmers.CardListConfirmers.AddSingleConflictConfirmer.OnConfirm = async (msg)
      => await DialogService.ShowAsync(root, new TwoButtonConfirmationDialog(msg.Title, msg.Message));
    confirmers.CardListConfirmers.ChangeCardPrintConfirmer.OnConfirm = async (msg)
      =>
    {
      Application.Current.Resources.TryGetValue("MTGPrintGridViewItemTemplate", out var template);

      return (await DialogService.ShowAsync(root, new GridViewDialog(
        title: msg.Title,
        items: msg.Data.ToArray(),
        itemTemplate: (DataTemplate)template))) as MTGCard;
    };
    confirmers.CardListConfirmers.AddCardGroupConfirmer.OnConfirm = async msg
      => await DialogService.ShowAsync(root, new TextBoxDialog(msg.Title)
      {
        InvalidInputCharacters = Path.GetInvalidFileNameChars(),
        PrimaryButtonText = "Add",
        InputValidation = input => !string.IsNullOrEmpty(input)
      });
    confirmers.CardListConfirmers.RenameCardGroupConfirmer.OnConfirm = async msg
      => await DialogService.ShowAsync(root, new TextBoxDialog(msg.Title)
      {
        InvalidInputCharacters = Path.GetInvalidFileNameChars(),
        InputText = msg.Data,
        PrimaryButtonText = "Rename",
        InputValidation = input => !string.IsNullOrEmpty(input)
      });
    confirmers.CardListConfirmers.MergeCardGroupsConfirmer.OnConfirm = async msg
      => await DialogService.ShowAsync(root, new TwoButtonConfirmationDialog(msg.Title, msg.Message)
      {
        PrimaryButtonText = "Merge"
      });

    confirmers.CommanderConfirmers.ChangeCardPrintConfirmer.OnConfirm = async (msg) =>
    {
      Application.Current.Resources.TryGetValue("MTGPrintGridViewItemTemplate", out var template);

      return (await DialogService.ShowAsync(root, new GridViewDialog(
        title: msg.Title,
        items: msg.Data.ToArray(),
        itemTemplate: (DataTemplate)template)
      {
        PrimaryButtonText = "Change",
      })) as MTGCard;
    };
  }
}