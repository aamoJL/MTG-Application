using MTGApplication.Features.DeckEditor.Views.Charts.Models;

namespace MTGApplication.Features.DeckEditor.Views.Charts.Views.Controls;

public sealed partial class ManaCurveChart
{
  partial class MTGCardCMCSeriesItem(int cmc) : MTGCardChartSeriesItem
  {
    public int CMC { get; } = cmc;
  }
}