using CommunityToolkit.Mvvm.Input;

namespace MTGApplication.General.ViewModels;

public interface ISavable
{
  public class ConfirmArgs()
  {
    public bool Canceled = false;
  }

  public bool HasUnsavedChanges { get; set; }
  
  IAsyncRelayCommand<ConfirmArgs> ConfirmUnsavedChangesCommand { get; }
}
