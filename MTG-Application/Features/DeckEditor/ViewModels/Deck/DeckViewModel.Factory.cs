using MTGApplication.Features.DeckEditor.Models;
using MTGApplication.General.Services.Databases.Repositories;
using MTGApplication.General.Services.Databases.Repositories.DeckRepository.Models;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplication.General.ViewModels;
using System;
using System.Threading.Tasks;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplication.Features.DeckEditor.ViewModels.Deck;

public partial class DeckViewModel
{
  public class Factory
  {
    public required Worker Worker { private get; init; }
    public required Notifier Notifier { private get; init; }
    public required IRepository<MTGCardDeckDTO> Repository { private get; init; }
    public required IMTGCardImporter Importer { private get; init; }
    public required DeckConfirmers DeckConfirmers { private get; init; }
    public required Func<Task> OnDeleted { private get; init; }

    public DeckViewModel Build(DeckEditorMTGDeck deck)
    {
      return new(deck)
      {
        Worker = Worker,
        Notifier = Notifier,
        Repository = Repository,
        Importer = Importer,
        Confirmers = DeckConfirmers,
        OnDeleted = OnDeleted,
      };
    }
  }
}