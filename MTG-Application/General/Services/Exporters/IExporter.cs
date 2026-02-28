using System.Threading.Tasks;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplication.General.Services.Exporters;

public interface IExporter<T>
{
  public Notification SuccessNotification { get; }

  public Task<bool> Export(T data);
}
