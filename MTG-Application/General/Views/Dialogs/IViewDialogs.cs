using static MTGApplication.General.Services.ConfirmationService.DialogService;

namespace MTGApplication.General.Views.Dialogs;

public interface IViewDialogs<T>
{
  public static abstract void RegisterConfirmDialogs(T confirmers, DialogWrapper wrapper);
}