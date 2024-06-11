using MTGApplication.Features.DeckEditor.Controls.Charts;

namespace MTGApplication.Features.DeckEditor;
public sealed partial class ManaCurveChart
{
  partial class MTGCardCMCSeriesItem(int cmc) : MTGCardChartSeriesItem()
  {
    public int CMC { get; } = cmc;
  }
}