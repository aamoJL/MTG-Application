namespace MTGApplication.Services.DialogService;

/// <summary>
/// Interface for root pages that can show dialogs
/// </summary>
public interface IDialogPresenter
{
  public DialogService.DialogWrapper DialogWrapper { get; }
}
