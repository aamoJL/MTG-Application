using System.Threading.Tasks;

namespace MTGApplication.Interfaces
{
  public interface ISavable
  {
    public bool HasUnsavedChanges { get; set; }

    public Task<bool> SaveUnsavedChanges();
  }
}
