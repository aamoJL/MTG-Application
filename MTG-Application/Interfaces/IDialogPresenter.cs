using MTGApplication.Services;

namespace MTGApplication.Interfaces;

/// <summary>
/// Interface for root pages that can show dialogs
/// </summary>
public interface IDialogPresenter
{
  public DialogService.DialogWrapper DialogWrapper { get; }
}
