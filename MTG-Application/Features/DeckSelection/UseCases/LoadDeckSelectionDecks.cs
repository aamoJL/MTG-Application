using Microsoft.EntityFrameworkCore;
using MTGApplication.Features.DeckSelector.Models;
using MTGApplication.General.Services.API.CardAPI;
using MTGApplication.General.Services.Databases.Repositories;
using MTGApplication.General.Services.Databases.Repositories.DeckRepository.Models;
using MTGApplication.General.Services.Databases.Repositories.DeckRepository.UseCases;
using MTGApplication.General.ViewModels;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace MTGApplication.Features.DeckSelector.UseCases;

public partial class LoadDeckSelectionDecks(IRepository<MTGCardDeckDTO> repository, MTGCardImporter importer) : UseCase<ObservableCollection<DeckSelectionDeck>, Task>
{
  public IWorker Worker { get; init; } = IWorker.Default;

  public override async Task Execute(ObservableCollection<DeckSelectionDeck> DeckItems)
  {
    var deckDTOs = await Worker.DoWork(new GetDeckDTOs(repository)
    {
      SetIncludes = (set) => { set.Include(x => x.Commander); },
    }.Execute());

    // TODO: save image to database so it does not need to be fetched here
    var commanders = (await Worker.DoWork(importer.ImportFromDTOs(deckDTOs.Where(x => x.Commander != null).Select(x => x.Commander)))).Found;

    var decks = deckDTOs.Select(d => new DeckSelectionDeck(
      Title: d.Name,
      ImageUri: commanders.FirstOrDefault(c => c.Info.ScryfallId == d.Commander.ScryfallId)?.Info.FrontFace.ImageUri ?? string.Empty));

    DeckItems.Clear();

    foreach (var deck in decks)
      DeckItems.Add(deck);
  }
}