using MTGApplication.General.Views.Dialogs;
using System;
using System.Linq;
using static MTGApplication.General.Services.ConfirmationService.DialogService;

namespace MTGApplication.Features.CardCollection;

public class CardCollectionViewDialogs : IViewDialogs<CardCollectionConfirmers>
{
  public static void RegisterConfirmDialogs(CardCollectionConfirmers confirmers, Func<DialogWrapper> getWrapper)
  {
    confirmers.SaveUnsavedChangesConfirmer.OnConfirm = async msg => await new ShowUnsavedChangesDialog(getWrapper.Invoke()).Execute((msg.Title, msg.Message));
    confirmers.LoadCollectionConfirmer.OnConfirm = async msg => await new ShowOpenDialog(getWrapper.Invoke()).Execute((msg.Title, msg.Message, msg.Data.ToArray()));
  }
}