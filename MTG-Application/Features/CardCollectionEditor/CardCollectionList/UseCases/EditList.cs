using MTGApplication.Features.CardCollectionEditor.CardCollectionList.Services;
using MTGApplication.Features.CardCollectionEditor.CardCollectionList.ViewModels;
using MTGApplication.Features.CardCollectionEditor.Editor.Services;
using MTGApplication.General.Services.NotificationService.UseCases;
using MTGApplication.General.ViewModels;
using System;
using System.Threading.Tasks;

namespace MTGApplication.Features.CardCollectionEditor.CardCollectionList.UseCases;

public partial class CardCollectionEditorViewModelCommands
{
  public class EditList(CardCollectionListViewModel viewmodel) : ViewModelAsyncCommand<CardCollectionListViewModel>(viewmodel)
  {
    protected override bool CanExecute() => !string.IsNullOrEmpty(Viewmodel.Name);

    protected override async Task Execute()
    {
      if (!CanExecute()) return;

      if (await Viewmodel.Confirmers.EditCollectionListConfirmer.Confirm(
        CardCollectionListConfirmers.GetEditCollectionListConfirmation((Viewmodel.Name, Viewmodel.Query)))
        is not (string name, string query) args)
        return;

      if (string.IsNullOrEmpty(name))
        new SendNotification(Viewmodel.Notifier).Execute(CardCollectionNotifications.EditListNameError);
      else if (string.IsNullOrEmpty(query))
        new SendNotification(Viewmodel.Notifier).Execute(CardCollectionNotifications.EditListQueryError);
      else if (Viewmodel.Name != name && Viewmodel.ExistsValidation.Invoke(name))
        new SendNotification(Viewmodel.Notifier).Execute(CardCollectionNotifications.EditListExistsError);
      else
      {
        Viewmodel.Name = name;
        Viewmodel.HasUnsavedChanges = true;

        await Viewmodel.UpdateQuery(query);

        new SendNotification(Viewmodel.Notifier).Execute(CardCollectionNotifications.EditListSuccess);
      }
    }
  }
}