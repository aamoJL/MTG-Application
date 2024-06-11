using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MTGApplication.Features.CardSearch.Models;
using MTGApplication.Features.CardSearch.Services;
using MTGApplication.General.Services.API.CardAPI;
using MTGApplication.General.ViewModels;
using static MTGApplication.Features.CardSearch.UseCases.CardSearchViewModelCommands;

namespace MTGApplication.Features.CardSearch;
/// <summary>
/// ViewModel for <see cref="CardSearchPage"/>
/// </summary>
public partial class CardSearchViewModel(MTGCardImporter importer) : ViewModelBase, IWorker
{
  public MTGCardImporter Importer { get; } = importer;
  public IncrementalLoadingCardCollection<CardSearchMTGCard> Cards { get; } = new(new CardSearchIncrementalCardSource(importer));
  public CardSearchConfirmers Confirmers { get; init; } = new();
  public IWorker Worker => this;

  [ObservableProperty] private bool isBusy;

  public IAsyncRelayCommand SubmitSearchCommand => new SubmitSearch(this).Command;
  public IAsyncRelayCommand<CardSearchMTGCard> ShowCardPrintsCommand => new ShowCardPrints(this).Command;
}