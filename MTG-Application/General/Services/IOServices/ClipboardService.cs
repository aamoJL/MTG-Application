using Windows.ApplicationModel.DataTransfer;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplication.General.Services.IOService;

/// <summary>
/// Service that handles clipboard actions
/// </summary>
public class ClipboardService
{
  public static Notification CopiedNotification { get; } = new(NotificationType.Info, "Copied to clipboard");

  /// <summary>
  /// Adds the <paramref name="text"/> to the clipboard
  /// </summary>
  public virtual void CopyToClipboard(string text)
  {
    DataPackage dataPackage = new();
    dataPackage.SetText(text);
    SetClipboardContent(dataPackage);
  }

  /// <summary>
  /// Sets the clipboard content to the <paramref name="dataPackage"/>
  /// </summary>
  protected virtual void SetClipboardContent(DataPackage dataPackage)
    => Clipboard.SetContent(dataPackage);
}
