namespace MTGApplication.General.ViewModels;

public interface ISavable
{
  public class ConfirmArgs()
  {
    public bool Cancelled = false;
  }

  public bool HasUnsavedChanges { get; set; }
}
