using MTGApplication.Features.CardCollection.Editor.ViewModels;
using MTGApplication.Features.CardCollectionEditor.CardCollectionList.Models;
using MTGApplication.Features.CardCollectionEditor.CardCollectionList.Services;
using MTGApplication.Features.CardCollectionEditor.Editor.Services;
using MTGApplication.General.Services.NotificationService.UseCases;
using MTGApplication.General.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MTGApplication.Features.CardCollectionEditor.CardCollectionList.UseCases;

public partial class CardCollectionEditorViewModelCommands
{
  public class EditList(CardCollectionEditorViewModel viewmodel) : ViewModelAsyncCommand<CardCollectionEditorViewModel, MTGCardCollectionList>(viewmodel)
  {
    protected override bool CanExecute(MTGCardCollectionList? list) => !string.IsNullOrEmpty(list?.Name);

    protected override async Task Execute(MTGCardCollectionList? list)
    {
      if (!CanExecute(list))
        return;

      if (await Viewmodel.Confirmers.CardCollectionListConfirmers.EditCollectionListConfirmer.Confirm(
        CardCollectionListConfirmers.GetEditCollectionListConfirmation((list!.Name, list.SearchQuery)))
        is not (string name, string query) args)
        return;

      if (string.IsNullOrEmpty(name))
        new SendNotification(Viewmodel.Notifier).Execute(CardCollectionNotifications.EditListNameError);
      else if (string.IsNullOrEmpty(query))
        new SendNotification(Viewmodel.Notifier).Execute(CardCollectionNotifications.EditListQueryError);
      else if (list.Name != name && Viewmodel.Collection.CollectionLists.FirstOrDefault(x => x.Name == name) is not null)
        new SendNotification(Viewmodel.Notifier).Execute(CardCollectionNotifications.EditListExistsError);
      else
      {
        if (query != list.SearchQuery)
        {
          try
          {
            // Fetch new query cards and remove cards that are not in the new query from the owned cards if the user accepts the conflict
            var result = await (Viewmodel as IWorker).DoWork(Viewmodel.Importer.ImportCardsWithSearchQuery(query, pagination: false));
            var found = result.Found;

            var excludedCards = list.Cards
              .ExceptBy(found.Select(f => f.Info.ScryfallId), o => o.Info.ScryfallId)
              .ToList();

            if (excludedCards.Count != 0)
              if (await Viewmodel.Confirmers.CardCollectionListConfirmers.EditCollectionListQueryConflictConfirmer
                .Confirm(CardCollectionListConfirmers.GetEditCollectionListQueryConflictConfirmation(excludedCards.Count))
                is not General.Services.ConfirmationService.ConfirmationResult.Yes)
                return; // Cancel edit if user cancels the conflict

            foreach (var item in excludedCards)
              list.Cards.Remove(item);

            list.SearchQuery = query;
          }
          catch (Exception e)
          {
            Viewmodel.Notifier.Notify(new(General.Services.NotificationService.NotificationService.NotificationType.Error, e.Message));
          }
        }

        if (name != list.Name)
          list.Name = name;

        new SendNotification(Viewmodel.Notifier).Execute(CardCollectionNotifications.EditListSuccess);
      }
    }
  }
}