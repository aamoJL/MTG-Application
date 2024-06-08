using System;
using System.Collections.Generic;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplication.Features.CardCollection.Services;

public class CardCollectionNotifications
{
  public static Notification OpenCollectionSuccess => new(NotificationType.Success, "The collection was loaded successfully.");
  public static Notification OpenCollectionError => new(NotificationType.Error, "Error. Could not load the collection.");

  public static Notification SaveCollectionSuccess => new(NotificationType.Success, "The list was saved successfully.");
  public static Notification SaveCollectionError => new(NotificationType.Error, "Error. Could not save the collection.");

  public static Notification DeleteCollectionSuccess => new(NotificationType.Success, "The list was deleted successfully.");
  public static Notification DeletecollectionError => new(NotificationType.Error, "Error. Could not delete the list.");

  public static Notification NewListSuccess => new(NotificationType.Success, "The list was added to the collection successfully.");
  public static Notification NewListNameError => new(NotificationType.Error, "Error. Name can't be empty.");
  public static Notification NewListQueryError => new(NotificationType.Error, "Error. Search query can't be empty.");
  public static Notification NewListExistsError => new(NotificationType.Error, "Error. List already exists in the collection.");

  public static Notification EditListSuccess => new(NotificationType.Success, "The list was changed successfully.");
  public static Notification EditListNameError => new(NotificationType.Error, "Error. Name can't be empty.");
  public static Notification EditListQueryError => new(NotificationType.Error, "Error. Search query can't be empty.");
  public static Notification EditListExistsError => new(NotificationType.Error, "Error. List already exists in the collection.");

  public static Notification DeleteListSuccess => new(NotificationType.Success, "The list was deleted successfully.");
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