using CommunityToolkit.Mvvm.ComponentModel;

namespace MTGApplication.General.ViewModels;

public partial class SaveStatus : ObservableObject
{
  public class ConfirmArgs()
  {
    public bool Cancelled = false;
  }

  [ObservableProperty] public partial bool HasUnsavedChanges { get; set; } = false;
}
