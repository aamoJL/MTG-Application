using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MTGApplication.General.Models.Card;
using MTGApplication.General.Services.API.CardAPI;
using MTGApplication.General.ViewModels;
using System.Threading.Tasks;

namespace MTGApplication.Features.CardSearch;
/// <summary>
/// ViewModel for <see cref="CardSearchPage"/>
/// </summary>
public partial class CardSearchViewModel : ViewModelBase, IWorker
{
  public CardSearchViewModel(ICardAPI<MTGCard> cardAPI)
  {
    CardAPI = cardAPI;
    Cards = new(CardAPI);
  }

  public ICardAPI<MTGCard> CardAPI { get; }
  public IncrementalLoadingCardCollection Cards { get; }

  public CardSearchConfirmers Confirmers { get; init; } = new();

  [ObservableProperty] private bool isBusy;

  [RelayCommand]
  private async Task SubmitSearch(string query)
  {
    var searchResult = await ((IWorker)this).DoWork(new FetchCards(CardAPI).Execute(query));

    Cards.SetCollection([.. searchResult.Found], searchResult.NextPageUri, searchResult.TotalCount);
  }

  [RelayCommand]
  private async Task ShowCardPrints(MTGCard card)
  {
    if (card == null)
      return;

    var prints = (await ((IWorker)this).DoWork(CardAPI.FetchFromUri(pageUri: card.Info.PrintSearchUri, paperOnly: true, fetchAll: true))).Found;

    await Confirmers.ShowCardPrintsConfirmer.Confirm(CardSearchConfirmers.GetShowCardPrintsConfirmation(prints));
  }
}