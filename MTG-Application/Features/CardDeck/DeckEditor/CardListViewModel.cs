﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MTGApplication.General.Models.Card;
using MTGApplication.General.Services.API.CardAPI;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace MTGApplication.Features.CardDeck;

public partial class CardListViewModel : ObservableObject
{
  public record MoveArgs(MTGCard Card, CardListViewModel Origin);

  public CardListViewModel(ICardAPI<MTGCard> cardAPI) => CardAPI = cardAPI;

  [ObservableProperty] private ObservableCollection<MTGCard> cards = new();

  public Action OnChange { get; init; }
  public ICardAPI<MTGCard> CardAPI { get; }

  [RelayCommand]
  private void AddCard(MTGCard card)
  {
    // TODO: validation
    Cards.Add(card);
    OnChange?.Invoke();
  }

  [RelayCommand]
  private void AddCards(MTGCard[] cards)
  {
    foreach (var card in cards)
      Cards.Add(card);

    OnChange?.Invoke();
  }

  [RelayCommand]
  private void RemoveCard(MTGCard card)
  {
    if (Cards.FirstOrDefault(x => x.Info.Name == card.Info.Name) is MTGCard existingCard)
    {
      Cards.Remove(existingCard);
      OnChange?.Invoke();
    }
  }

  [RelayCommand]
  private void MoveCard(MoveArgs args)
  {
    var (card, origin) = args;

    AddCard(card);
    origin.RemoveCard(card);
  }

  [RelayCommand]
  private async Task ImportCards(string data)
  {
    var fetchResult = await CardAPI.FetchFromString(data);

    AddCards(fetchResult.Found);
  }

  [RelayCommand] private void CardlistCardChanged() => OnChange?.Invoke();
}