using MTGApplication.Features.DeckEditor.Models;
using MTGApplication.General.Services.Exporters;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplication.General.Services.Importers.CardImporter.ScryfallAPI;
using MTGApplication.General.Services.IOServices;
using MTGApplication.General.Services.ReversibleCommandService;
using MTGApplication.General.ViewModels;
using System.Collections.ObjectModel;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplication.Features.DeckEditor.ViewModels.DeckCardList;

public partial class DeckCardListViewModel
{
  public class Factory
  {
    public required Worker Worker { protected get; init; }
    public required ReversibleCommandStack UndoStack { protected get; init; }
    public required Notifier Notifier { protected get; init; }
    public required IMTGCardImporter Importer { protected get; init; }
    public required IEdhrecImporter EdhrecImporter { protected get; init; }
    public required IScryfallImporter ScryfallImporter { protected get; init; }
    public required IExporter<string> Exporter { protected get; init; }
    public required INetworkService NetworkService { protected get; init; }
    public required CardListConfirmers ListConfirmers { protected get; init; }

    public DeckCardListViewModel Build(ObservableCollection<DeckEditorMTGCard> list)
    {
      return new(list)
      {
        Worker = Worker,
        Importer = Importer,
        EdhrecImporter = EdhrecImporter,
        ScryfallImporter = ScryfallImporter,
        Exporter = Exporter,
        UndoStack = UndoStack,
        Notifier = Notifier,
        NetworkService = NetworkService,
        Confirmers = ListConfirmers,
      };
    }
  }
}
