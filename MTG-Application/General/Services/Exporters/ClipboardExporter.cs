using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplication.General.Services.Exporters;

/// <summary>
/// Service that handles clipboard actions
/// </summary>
public class ClipboardExporter : IExporter<string>
{
  public Notification SuccessNotification => new(NotificationType.Success, "Copied to clipboard");

  public async Task<bool> Export(string data)
  {
    CopyToClipboard(data);
    return true;
  }

  /// <summary>
  /// Adds the <paramref name="text"/> to the clipboard
  /// </summary>
  public virtual void CopyToClipboard(string text)
  {
    DataPackage dataPackage = new();
    dataPackage.SetText(text);
    Clipboard.SetContent(dataPackage);
  }
}