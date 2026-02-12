using System;

namespace MTGApplication.General.ViewModels;

// TODO: remove
[Obsolete]
public interface ISavable
{
  public bool HasUnsavedChanges { get; }
}