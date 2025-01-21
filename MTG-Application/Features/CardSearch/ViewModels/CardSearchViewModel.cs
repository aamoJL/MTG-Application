using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MTGApplication.Features.CardSearch.Models;
using MTGApplication.Features.CardSearch.Services;
using MTGApplication.Features.CardSearch.Views;
using MTGApplication.General.Models;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplication.General.ViewModels;
using static MTGApplication.Features.CardSearch.UseCases.CardSearchViewModelCommands;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplication.Features.CardSearch.ViewModels;
/// <summary>
/// ViewModel for <see cref="CardSearchPage"/>
/// </summary>
public partial class CardSearchViewModel(IMTGCardImporter importer) : ViewModelBase, IWorker
{
  public IMTGCardImporter Importer { get; } = importer;
  public IncrementalLoadingCardCollection<MTGCard> Cards { get; } = new(new CardSearchIncrementalCardSource(importer));
  public CardSearchConfirmers Confirmers { get; init; } = new();
  public IWorker Worker => this;
  public Notifier Notifier { get; init; } = new();

  [ObservableProperty] public partial bool IsBusy { get; set; }

  public IAsyncRelayCommand? SubmitSearchCommand => field ??= new SubmitSearch(this).Command;
  public IAsyncRelayCommand<MTGCard>? ShowCardPrintsCommand => field ??= new ShowCardPrints(this).Command;
}