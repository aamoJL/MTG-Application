using System;

namespace MTGApplication.General.Views.Controls;

public partial class SelectionModel
{
  public int SelectionIndex
  {
    get;
    set
    {
      if (field != value)
      {
        field = value;
        SelectionChanged?.Invoke(this, value);
      }
    }
  } = -1;

  public event EventHandler<int>? SelectionChanged;
}