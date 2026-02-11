using CommunityToolkit.Mvvm.ComponentModel;

namespace MTGApplication.General.ViewModels;

public partial class SaveStatus : ObservableObject
{
  [ObservableProperty] public partial bool HasUnsavedChanges { get; set; } = false;
}
