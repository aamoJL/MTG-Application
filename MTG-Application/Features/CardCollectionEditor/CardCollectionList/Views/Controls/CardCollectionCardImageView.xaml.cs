namespace MTGApplication.Features.CardCollectionEditor.CardCollectionList.Views.Controls;
public sealed partial class CardCollectionCardImageView : CardCollectionCardViewBase
{
  public double OwnedToOpacity(bool owned) => owned ? 1 : .5f;

  public CardCollectionCardImageView() => InitializeComponent();
}