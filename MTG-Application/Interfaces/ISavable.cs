using System.Threading.Tasks;

namespace MTGApplication.Interfaces;

/// <summary>
/// Interface for savable objects
/// </summary>
public interface ISavable
{
  public bool HasUnsavedChanges { get; set; }

  /// <summary>
  /// Saves the unsaved changes in this object
  /// </summary>
  public Task<bool> SaveUnsavedChanges();
}