using System;
using static MTGApplication.Services.DialogService;

namespace MTGApplication.Interfaces;

public interface IDialogNotifier
{
  public event EventHandler<DialogEventArgs> OnGetDialogWrapper;

  public DialogWrapper GetDialogWrapper();
}
