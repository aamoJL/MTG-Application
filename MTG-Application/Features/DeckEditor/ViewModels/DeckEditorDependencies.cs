using MTGApplication.Features.DeckEditor.ViewModels.Deck;
using MTGApplication.Features.DeckEditor.ViewModels.DeckCard;
using MTGApplication.Features.DeckEditor.ViewModels.DeckCardGroup.CardGroup;
using MTGApplication.Features.DeckEditor.ViewModels.DeckCardGroup.GroupedCardList;
using MTGApplication.Features.DeckEditor.ViewModels.DeckCardList;
using MTGApplication.General.Services.API.CardAPI;
using MTGApplication.General.Services.Databases.Repositories;
using MTGApplication.General.Services.Databases.Repositories.DeckRepository;
using MTGApplication.General.Services.Databases.Repositories.DeckRepository.Models;
using MTGApplication.General.Services.Exporters;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplication.General.Services.Importers.CardImporter.ScryfallAPI;
using MTGApplication.General.Services.IOServices;
using MTGApplication.General.ViewModels;
using static MTGApplication.Features.DeckEditor.ViewModels.EditorPage.DeckEditorPageViewModel;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplication.Features.DeckEditor.ViewModels;

public class DeckEditorDependencies
{
  public Worker Worker { get; init; } = new();
  public IRepository<MTGCardDeckDTO> Repository { get; init; } = new DeckDTORepository();
  public IMTGCardImporter Importer { get; init; } = App.MTGCardImporter;
  public IEdhrecImporter EdhrecImporter { get; init; } = new EdhrecImporter();
  public IScryfallImporter ScryfallImporter { get; init; } = new ScryfallAPI();
  public IExporter<string> Exporter { get; init; } = new ClipboardExporter();
  public Notifier Notifier { get; init; } = new();
  public INetworkService NetworkService { get; init; } = new NetworkService();
  public EditorPageConfirmers PageConfirmers { get; init; } = new();
  public DeckViewModel.DeckConfirmers DeckConfirmers { get; init; } = new();
  public DeckCardListViewModel.CardListConfirmers ListConfirmers { get; init; } = new();
  public DeckCardViewModel.CardConfirmers CardConfirmers { get; init; } = new();
  public GroupedDeckCardListViewModel.GroupedCardListConfirmers GroupListConfirmers { get; init; } = new();
  public DeckCardGroupViewModel.GroupConfirmers GroupConfirmers { get; init; } = new();
}