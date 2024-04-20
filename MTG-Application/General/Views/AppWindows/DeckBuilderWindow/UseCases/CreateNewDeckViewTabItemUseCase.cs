using Microsoft.UI.Xaml.Controls;

namespace MTGApplication.General.Views;
/// <summary>
/// Use case to create new tab item with <see cref=""/> as the content
/// </summary>
public class CreateNewDeckViewTabItemUseCase : UseCase<TabViewItem>
{
  public override TabViewItem Execute()
  {
    //var content = new DeckBuilderTabFrame(CardPreviewProperties).Init();
    var frame = new Frame();

    return new TabViewItem()
    {
      Header = new TextBlock() { Text = "New tab" }, //new DeckBuilderTabHeaderControl(content),
      Content = frame,
    };
  }
}
