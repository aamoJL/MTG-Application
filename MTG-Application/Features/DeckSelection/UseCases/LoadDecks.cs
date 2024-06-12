using Microsoft.EntityFrameworkCore;
using MTGApplication.Features.DeckSelection.Models;
using MTGApplication.General.Services.Databases.Repositories.DeckRepository.UseCases;
using MTGApplication.General.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace MTGApplication.Features.DeckSelection.UseCases;

public partial class DeckSelectorViewModelCommands
{
  public class LoadDecks(DeckSelectionViewModel viewmodel) : ViewModelAsyncCommand<DeckSelectionViewModel>(viewmodel)
  {
    protected override async Task Execute()
    {
      var deckDTOs = await Viewmodel.Worker.DoWork(new GetDeckDTOs(Viewmodel.Repository)
      {
        SetIncludes = (set) => { set.Include(x => x.Commander).Load(); },
      }.Execute());

      // TODO: save image to database so it does not need to be fetched here
      var commanders = (await Viewmodel.Worker.DoWork(Viewmodel.Importer.ImportFromDTOs(deckDTOs.Where(x => x.Commander != null).Select(x => x.Commander)))).Found;

      var decks = deckDTOs.Select(d => new DeckSelectionDeck(
        Title: d.Name,
        ImageUri: commanders.FirstOrDefault(c => c.Info.ScryfallId == d.Commander.ScryfallId)?.Info.FrontFace.ImageUri ?? string.Empty));

      Viewmodel.DeckItems.Clear();

      foreach (var deck in decks)
        Viewmodel.DeckItems.Add(deck);
    }
  }
}