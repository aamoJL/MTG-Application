using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MTGApplication.Features.CardSearch.Models;
using MTGApplication.Features.CardSearch.Services;
using MTGApplication.General.Models;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplication.General.ViewModels;
using static MTGApplication.Features.CardSearch.UseCases.CardSearchViewModelCommands;

namespace MTGApplication.Features.CardSearch;
/// <summary>
/// ViewModel for <see cref="CardSearchPage"/>
/// </summary>
public partial class CardSearchViewModel(MTGCardImporter importer) : ViewModelBase, IWorker
{
  public MTGCardImporter Importer { get; } = importer;
  public IncrementalLoadingCardCollection<MTGCard> Cards { get; } = new(new CardSearchIncrementalCardSource(importer));
  public CardSearchConfirmers Confirmers { get; init; } = new();
  public IWorker Worker => this;

  [ObservableProperty] private bool isBusy;

  public IAsyncRelayCommand SubmitSearchCommand => new SubmitSearch(this).Command;
  public IAsyncRelayCommand<MTGCard> ShowCardPrintsCommand => new ShowCardPrints(this).Command;
}