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
        if (query != Viewmodel.Query)
        {
          // Fetch new query cards and remove cards that are not in the new query from the owned cards if the user accepts the conflict
          var result = await Viewmodel.Worker.DoWork(Viewmodel.Importer.ImportCardsWithSearchQuery(query, pagination: false));
          var found = result.Found;

          var excludedCards = Viewmodel.OwnedCards
            .ExceptBy(found.Select(f => f.Info.ScryfallId), o => o.Info.ScryfallId)
            .ToList();

          if (excludedCards.Count != 0)
            if (await Viewmodel.Confirmers.EditCollectionListQueryConflictConfirmer
              .Confirm(CardCollectionListConfirmers.GetEditCollectionListQueryConflictConfirmation(excludedCards.Count))
              is not General.Services.ConfirmationService.ConfirmationResult.Yes)
              return; // Cancel edit if user cancels the conflict

          foreach (var item in excludedCards)
            Viewmodel.OwnedCards.Remove(item);

          Viewmodel.Query = query;

          await Viewmodel.Worker.DoWork(Viewmodel.UpdateQueryCards());
        }

        if (name != Viewmodel.Name)
          Viewmodel.Name = name;

        Viewmodel.HasUnsavedChanges = true;

        new SendNotification(Viewmodel.Notifier).Execute(CardCollectionNotifications.EditListSuccess);
      }
    }
  }
}