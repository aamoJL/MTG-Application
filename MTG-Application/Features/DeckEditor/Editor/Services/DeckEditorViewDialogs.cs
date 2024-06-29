﻿using MTGApplication.General.Models;
using MTGApplication.General.Views.Dialogs;
using MTGApplication.General.Views.Dialogs.Controls;
using MTGApplication.General.Views.Dialogs.UseCases;
using System.Linq;
using static MTGApplication.General.Services.ConfirmationService.DialogService;

namespace MTGApplication.Features.DeckEditor.Editor.Services;

public class DeckEditorViewDialogs : IViewDialogs<DeckEditorConfirmers>
{
  public static void RegisterConfirmDialogs(DeckEditorConfirmers confirmers, DialogWrapper wrapper)
  {
    confirmers.SaveUnsavedChangesConfirmer.OnConfirm = async msg => await new ShowUnsavedChangesDialog(wrapper).Execute((msg.Title, msg.Message));
    confirmers.LoadDeckConfirmer.OnConfirm = async msg => await new ShowOpenDialog(wrapper).Execute((msg.Title, msg.Message, msg.Data));
    confirmers.SaveDeckConfirmer.OnConfirm = async msg => await new ShowSaveDialog(wrapper).Execute((msg.Title, msg.Message, msg.Data));
    confirmers.OverrideDeckConfirmer.OnConfirm = async msg => await new ShowOverrideDialog(wrapper).Execute((msg.Title, msg.Message));
    confirmers.DeleteDeckConfirmer.OnConfirm = async msg => await new ShowDeleteDialog(wrapper).Execute((msg.Title, msg.Message));
    confirmers.ShowTokensConfirmer.OnConfirm = async (msg)
      => await new GridViewDialog<MTGCard>(msg.Title, "MTGPrintGridViewItemTemplate", "MTGAdaptiveGridViewStyle")
      {
        Items = msg.Data.ToArray(),
        PrimaryButtonText = string.Empty,
        SecondaryButtonText = string.Empty,
        CloseButtonText = "Close"
      }.ShowAsync(wrapper) as MTGCard;

    confirmers.CardListConfirmers.ExportConfirmer.OnConfirm = async msg
      => await wrapper.ShowAsync(new TextAreaDialog(msg.Title)
      {
        InputText = msg.Data,
        PrimaryButtonText = "Copy to Clipboard",
      });
    confirmers.CardListConfirmers.ImportConfirmer.OnConfirm = async msg
      => await wrapper.ShowAsync(new TextAreaDialog(msg.Title)
      {
        InputText = msg.Data,
        InputPlaceholderText = "Example:\n2 Black Lotus\nMox Ruby\nbd8fa327-dd41-4737-8f19-2cf5eb1f7cdd",
        PrimaryButtonText = "Import",
      });
    confirmers.CardListConfirmers.AddMultipleConflictConfirmer.OnConfirm = async (msg)
      => await wrapper.ShowAsync(new CheckBoxDialog(msg.Title, msg.Message)
      {
        InputText = "Same for all cards.",
        CloseButtonText = "No",
      });
    confirmers.CardListConfirmers.AddSingleConflictConfirmer.OnConfirm = async (msg)
      => await wrapper.ShowAsync(new TwoButtonConfirmationDialog(msg.Title, msg.Message));
    confirmers.CardListConfirmers.ChangeCardPrintConfirmer.OnConfirm = async (msg)
      => await new GridViewDialog<MTGCard>(msg.Title, "MTGPrintGridViewItemTemplate", "MTGAdaptiveGridViewStyle")
      {
        Items = msg.Data.ToArray(),
        SecondaryButtonText = string.Empty
      }.ShowAsync(wrapper) as MTGCard;
    confirmers.CommanderConfirmers.ChangeCardPrintConfirmer.OnConfirm = async (msg)
      => await new GridViewDialog<MTGCard>(msg.Title, "MTGPrintGridViewItemTemplate", "MTGAdaptiveGridViewStyle")
      {
        Items = msg.Data.ToArray(),
        PrimaryButtonText = "Change",
        SecondaryButtonText = string.Empty,
      }.ShowAsync(wrapper) as MTGCard;
  }
}