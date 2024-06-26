﻿using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplication.Features.DeckEditor.Commanders.Services;

public static class CommanderNotifications
{
  public static Notification ImportSuccess => new(NotificationType.Success, "Commander was imported successfully.");
  public static Notification ImportNotLegendaryError => new(NotificationType.Error, "Error. Commander needs to be Legendary.");
  public static Notification ImportError => new(NotificationType.Error, "Error. Could not import the card.");
}
