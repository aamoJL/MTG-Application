namespace MTGApplication.Features.CardDeck;

public partial class DeckSelectorViewModel
{
  public class DeckSelectedEventArgs : System.EventArgs
  {
    public DeckSelectedEventArgs(string name) => Name = name ?? "";

    public string Name { get; }
  }
}
