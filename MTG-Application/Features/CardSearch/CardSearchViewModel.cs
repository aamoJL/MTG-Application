using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MTGApplication.Features.CardSearch.Services;
using MTGApplication.General.Models.Card;
using MTGApplication.General.Services.API.CardAPI;
using MTGApplication.General.ViewModels;
using System.Threading.Tasks;

namespace MTGApplication.Features.CardSearch;
/// <summary>
/// ViewModel for <see cref="CardSearchPage"/>
/// </summary>
public partial class CardSearchViewModel(ICardImporter<DeckEditorMTGCard> cardAPI) : ViewModelBase, IWorker
{
  public ICardImporter<DeckEditorMTGCard> CardAPI { get; } = cardAPI;
  public IncrementalLoadingCardCollection<DeckEditorMTGCard> Cards { get; } = new(new DefaultIncrementalCardSource(cardAPI));

  public CardSearchConfirmers Confirmers { get; init; } = new();

  [ObservableProperty] private bool isBusy;

  [RelayCommand]
  private async Task SubmitSearch(string query)
  {
    var searchResult = await ((IWorker)this).DoWork(new FetchCards(CardAPI).Execute(query));

    Cards.SetCollection([.. searchResult.Found], searchResult.NextPageUri, searchResult.TotalCount);
  }

  [RelayCommand]
  private async Task ShowCardPrints(DeckEditorMTGCard card)
  {
    if (card == null)
      return;

    var prints = (await ((IWorker)this).DoWork(CardAPI.ImportFromUri(pageUri: card.Info.PrintSearchUri, paperOnly: true, fetchAll: true))).Found;

    await Confirmers.ShowCardPrintsConfirmer.Confirm(CardSearchConfirmers.GetShowCardPrintsConfirmation(prints));
  }
}