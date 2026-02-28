using System;
using System.Collections.Generic;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplication.Features.CardCollectionEditor.ViewModels.CollectionList;

public partial class CardCollectionListViewModel
{
  public static class Notifications
  {
    public static Notification EditListSuccess => new(NotificationType.Success, "The list was changed successfully.");
    public static Notification EditListNameError => new(NotificationType.Error, "Error. Name can't be empty.");
    public static Notification EditListQueryError => new(NotificationType.Error, "Error. Search query can't be empty.");
    public static Notification EditListExistsError => new(NotificationType.Error, "Error. List already exists in the collection.");

    public static Notification DeleteListError => new(NotificationType.Error, "Error. Could not delete list.");

    public static Notification ImportCardsSuccessOrWarning(int added, int skipped, int notFound)
    {
      var messages = new List<string>();

      if (added > 0) messages.Add($"{added} cards were imported successfully.");
      if (skipped > 0) messages.Add($"{skipped} cards were skipped");
      if (notFound > 0) messages.Add($"{notFound} cards were not found.");

      var notificationType = notFound == 0 ? NotificationType.Success : NotificationType.Warning;

      return new(notificationType, string.Join(Environment.NewLine, messages));
    }
    public static Notification ImportCardsError => new(NotificationType.Error, $"Error. No cards were imported.");
  }
}
