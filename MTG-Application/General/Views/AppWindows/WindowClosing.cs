using Microsoft.UI.Xaml;
using MTGApplication.General.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MTGApplication.General.Views.AppWindows;

public class WindowClosing(XamlRoot root)
{
  public class ClosingEventArgs(XamlRoot root) : EventArgs
  {
    public XamlRoot Root { get; } = root;
    public List<Func<ISavable.ConfirmArgs, Task>> Tasks { get; } = [];
  };
  public class ClosedEventArgs(XamlRoot root) : EventArgs { public XamlRoot Root { get; } = root; }

  public static event EventHandler<ClosingEventArgs>? Closing;
  public static event EventHandler<ClosedEventArgs>? Closed;

  public XamlRoot Root { get; } = root;

  public async Task<bool> Close()
  {
    var closingArgs = new ClosingEventArgs(Root);

    Closing?.Invoke(null, closingArgs);

    var savableConfirmArgs = new ISavable.ConfirmArgs();

    foreach (var task in closingArgs.Tasks)
    {
      await task.Invoke(savableConfirmArgs);

      if (savableConfirmArgs.Cancelled)
        break;
    }

    if (!savableConfirmArgs.Cancelled)
      Closed?.Invoke(null, new(Root));

    return !savableConfirmArgs.Cancelled;
  }
}
