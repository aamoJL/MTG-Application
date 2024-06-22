using MTGApplication.Features.DeckTesting.Models;
using MTGApplication.General.Views.Controls;

namespace MTGApplication.Features.DeckTesting.Views.Controls.CardView;
public sealed partial class DeckTestingCardTextView : DeckTestingCardViewBase
{
  public DeckTestingCardTextView() => InitializeComponent();
}

public partial class DeckTestingCardViewBase : BasicCardView<DeckTestingMTGCard>
{

}
