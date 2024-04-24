using System;

namespace MTGApplication.General.Services.ConfirmationService;

/// <summary>
/// Interface for classes that want to show content dialogs
/// </summary>
public interface IDialogNotifier
{
  /// <summary>
  /// Event, that asks a View to ask the Root to give its <see cref="DialogWrapper"/>
  /// </summary>
  public event EventHandler<DialogService.DialogEventArgs> OnGetDialogWrapper;

  /// <summary>
  /// Invokes <see cref="OnGetDialogWrapper"/> event
  /// </summary>
  public DialogService.DialogWrapper GetDialogWrapper();
}
