using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MTGApplication.General.Models.Card;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace MTGApplication.Features.DeckEditor;

public partial class CardListViewModel : ObservableObject
{
  public CardListViewModel(CardImporter cardImporter) => CardImporter = cardImporter;

  [ObservableProperty] private ObservableCollection<MTGCard> cards = new();

  public CardImporter CardImporter { get; }

  public Action OnChange { get; init; }

  [RelayCommand]
  private void AddCard(MTGCard card)
  {
    if (Cards.FirstOrDefault(x => x.Info.Name == card.Info.Name) is MTGCard existingCard)
      existingCard.Count += card.Count;
    else
      Cards.Add(card);

    OnChange?.Invoke();
  }

  [RelayCommand]
  private void AddCards(MTGCard[] cards)
  {
    foreach (var card in cards)
      AddCard(card);
  }

  [RelayCommand]
  private async Task ImportCards(string data)
  {
    var result = await CardImporter.Import(data);

    foreach (var card in result.Found)
      AddCard(card);
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

  [RelayCommand] private void CardlistCardChanged() => OnChange?.Invoke();
}