using MTGApplication.General.Services.Exporters;
using MTGApplication.General.Services.NotificationService;

namespace MTGApplicationTests.TestUtility.Exporters;

public partial class TestStringExporter : IExporter<string>
{
  public NotificationService.Notification SuccessNotification => new(NotificationService.NotificationType.Success, "Success");

  public bool? Response { get; set; } = null;
  public string Result { get; private set; } = null;

  public async Task<bool> Export(string data)
  {
    if (!Response.HasValue)
      throw new NullReferenceException(nameof(Response));

    Result = data;

    return await Task.FromResult(Response.Value);
  }
}
