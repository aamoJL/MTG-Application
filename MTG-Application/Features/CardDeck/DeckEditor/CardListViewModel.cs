using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MTGApplication.General.Models.Card;
using System;
using System.Collections.ObjectModel;

namespace MTGApplication.Features.CardDeck;

public partial class CardListViewModel : ObservableObject
{
  [ObservableProperty] private ObservableCollection<MTGCard> cards = new();

  public Action OnChange { get; init; }

  [RelayCommand]
  private void RemoveCard(MTGCard card)
  {
    Cards.Remove(card);
    OnChange?.Invoke();
  }

  [RelayCommand] private void CardlistCardChanged() => OnChange?.Invoke();
}