namespace MTGApplication.Features.CardDeck;
public class MTGDeckSelectionListItem
{
  public MTGDeckSelectionListItem(string title, string imageUri = "")
  {
    Title = title;
    ImageUri = imageUri;
  }

  public string Title { get; }
  public string ImageUri { get; }
}
