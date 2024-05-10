using System.Threading.Tasks;

namespace MTGApplication.General.ViewModels;

public interface ISavable
{
  public bool HasUnsavedChanges { get; set; }

  public Task<bool> ConfirmUnsavedChanges();
}
