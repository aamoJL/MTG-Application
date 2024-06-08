﻿namespace MTGApplication.Features.DeckSelector.Models;
public class DeckSelectionListItem
{
  public DeckSelectionListItem(string title, string imageUri = "")
  {
    Title = title;
    ImageUri = imageUri;
  }

  public string Title { get; }
  public string ImageUri { get; }
}
