using CommunityToolkit.Mvvm.Input;
using MTGApplication.Features.DeckEditor.Models;
using Windows.ApplicationModel.DataTransfer.DragDrop;

namespace MTGApplication.General.Views.DragAndDrop;

public class CardDragArgs(object item, object origin)
{
  public interface IMoveOrigin
  {
    public IRelayCommand<DeckEditorMTGCard>? OnDropBeginMoveFrom { get; set; }
    public IRelayCommand? OnDropExecuteMove { get; set; }
  }

  public static DragDropModifiers MoveModifier => DragDropModifiers.Shift;

  public object Origin { get; } = origin;
  public object Item { get; } = item;
}
