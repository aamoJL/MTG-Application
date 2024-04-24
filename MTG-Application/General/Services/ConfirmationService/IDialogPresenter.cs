namespace MTGApplication.General.Services.ConfirmationService;

/// <summary>
/// Interface for root pages that can show dialogs
/// </summary>
public interface IDialogPresenter
{
  public DialogService.DialogWrapper DialogWrapper { get; }
}
