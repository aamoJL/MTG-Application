using MTGApplication.Features.CardCollection.Editor.ViewModels;
using MTGApplication.Features.CardCollectionEditor.CardCollection.Services;
using MTGApplication.Features.CardCollectionEditor.CardCollectionList.Models;
using MTGApplication.Features.CardCollectionEditor.Editor.Services;
using MTGApplication.General.Services.NotificationService.UseCases;
using MTGApplication.General.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MTGApplication.Features.CardCollection.Editor.UseCases;

public partial class CardCollectionEditorViewModelCommands
{
  public class ConfirmNewList(CardCollectionEditorViewModel viewmodel) : AsyncCommand
  {
    protected override async Task Execute()
    {
      if (await viewmodel.Confirmers.CardCollectionConfirmers.NewCollectionListConfirmer.Confirm(CardCollectionConfirmers.GetNewCollectionListConfirmation())
      is not (string name, string query))
        return;

      var errorNotification =
        string.IsNullOrEmpty(name) ? CardCollectionNotifications.NewListNameError :
        string.IsNullOrEmpty(query) ? CardCollectionNotifications.NewListQueryError :
        viewmodel.CollectionLists.FirstOrDefault(x => x.Name == name) is not null ? CardCollectionNotifications.NewListExistsError : null;

      if (errorNotification != null)
      {
        new SendNotification(viewmodel.Notifier).Execute(errorNotification);
        return;
      }

      try
      {
        await viewmodel.Worker.DoWork(AddNewList(name, query));
        new SendNotification(viewmodel.Notifier).Execute(CardCollectionNotifications.NewListSuccess);
      }
      catch { }
    }

    private async Task AddNewList(string name, string query)
    {
      var newList = new MTGCardCollectionList() { Name = name, SearchQuery = query };

      viewmodel.CollectionLists.Add(newList);
      await viewmodel.SelectedCardCollectionListViewModel.ChangeCollectionList(newList);
    }
  }
}