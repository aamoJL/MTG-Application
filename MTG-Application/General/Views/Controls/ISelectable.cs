namespace MTGApplication.General.Views.Controls;

public interface ISelectable
{
  bool IsSelected { get; set; }
  int SelectionIndex { get; set; }
}
