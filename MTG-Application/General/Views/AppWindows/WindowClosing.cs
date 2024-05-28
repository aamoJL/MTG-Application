using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MTGApplication.General.Views.AppWindows;

public class WindowClosing(XamlRoot root)
{
  public class ClosingEventArgs(XamlRoot root) : EventArgs
  {
    public XamlRoot Root { get; } = root;
    public List<Func<Task<bool>>> Tasks { get; } = [];
  };
  public class ClosedEventArgs(XamlRoot root) : EventArgs { public XamlRoot Root { get; } = root; }

  public static event EventHandler<ClosingEventArgs> Closing;
  public static event EventHandler<ClosedEventArgs> Closed;
  
  public XamlRoot Root { get; } = root;

  public async Task<bool> Close()
  {
    var cancelled = false;
    var closingArgs = new ClosingEventArgs(Root);
    
    Closing?.Invoke(null, closingArgs);

    foreach (var task in closingArgs.Tasks)
    {
      if (!await task.Invoke())
      {
        cancelled = true; 
        break;
      }
    }

    if(!cancelled)
      Closed?.Invoke(null, new(Root));

    return !cancelled;
  }
}
