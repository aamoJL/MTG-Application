using MTGApplication.Features.CardCollectionEditor.CardCollectionList.Models;
using MTGApplication.Features.CardCollectionEditor.CardCollectionList.Services;
using MTGApplication.Features.CardCollectionEditor.CardCollectionList.ViewModels;
using MTGApplication.Features.CardCollectionEditor.Editor.Services;
using MTGApplication.General.Services.NotificationService.UseCases;
using MTGApplication.General.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MTGApplication.Features.CardCollectionEditor.CardCollectionList.UseCases;

public partial class CardCollectionEditorViewModelCommands
{
  public class EditList(CardCollectionListViewModel viewmodel) : AsyncCommand
  {
    protected override bool CanExecute() => !string.IsNullOrEmpty(viewmodel.Name);

    protected override async Task Execute()
    {
      if (!CanExecute())
        return;

      if (await viewmodel.Confirmers.EditCollectionListConfirmer.Confirm(
        CardCollectionListConfirmers.GetEditCollectionListConfirmation((viewmodel.Name, viewmodel.Query)))
        is not (string name, string query) args)
        return;

      var errorNotification = string.IsNullOrEmpty(name) ? CardCollectionNotifications.EditListNameError :
        string.IsNullOrEmpty(query) ? CardCollectionNotifications.EditListQueryError :
        viewmodel.Name != name && viewmodel.NameValidator.Invoke(name) != true ? CardCollectionNotifications.EditListExistsError : null;

      if (errorNotification != null)
      {
        new SendNotification(viewmodel.Notifier).Execute(errorNotification);
        return;
      }

      try
      {
        await viewmodel.Worker.DoWork(Edit(viewmodel.CollectionList, name, query));
        await viewmodel.ChangeCollectionList(viewmodel.CollectionList);

        new SendNotification(viewmodel.Notifier).Execute(CardCollectionNotifications.EditListSuccess);
      }
      catch (Exception e)
      {
        new SendNotification(viewmodel.Notifier).Execute(new(General.Services.NotificationService.NotificationService.NotificationType.Error, e.Message));
      }
    }

    private async Task Edit(MTGCardCollectionList list, string name, string query)
    {
      if (query != list.SearchQuery)
      {
        try
        {
          // Fetch new query cards and remove cards that are not in the new query from the owned cards if the user accepts the conflict
          var result = await viewmodel.Worker.DoWork(viewmodel.Importer.ImportCardsWithSearchQuery(query, pagination: false));
          var found = result.Found;

          var excludedCards = list.Cards
            .ExceptBy(found.Select(f => f.Info.ScryfallId), o => o.Info.ScryfallId)
            .ToList();

          if (excludedCards.Count != 0)
            if (await viewmodel.Confirmers.EditCollectionListQueryConflictConfirmer
              .Confirm(CardCollectionListConfirmers.GetEditCollectionListQueryConflictConfirmation(excludedCards.Count))
              is not General.Services.ConfirmationService.ConfirmationResult.Yes)
              return; // Cancel edit if user cancels the conflict

          foreach (var item in excludedCards)
            list.Cards.Remove(item);

          list.SearchQuery = query;
        }
        catch (Exception e)
        {
          viewmodel.Notifier.Notify(new(General.Services.NotificationService.NotificationService.NotificationType.Error, e.Message));
        }
      }

      if (name != list.Name)
        list.Name = name;
    }
  }
}