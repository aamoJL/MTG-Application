using MTGApplication.Features.CardSearch.Models;
using MTGApplication.Features.CardSearch.Services;
using MTGApplication.General.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace MTGApplication.Features.CardSearch.UseCases;

public partial class CardSearchViewModelCommands
{
  public class ShowCardPrints(CardSearchViewModel viewmodel) : ViewModelAsyncCommand<CardSearchViewModel, CardSearchMTGCard>(viewmodel)
  {
    protected override async Task Execute(CardSearchMTGCard card)
    {
      if (card == null) return;
      var prints = (await Viewmodel.Worker.DoWork(Viewmodel.Importer.ImportFromUri(pageUri: card.Info.PrintSearchUri, paperOnly: true, fetchAll: true))).Found.Select(x => x.Info);

      await Viewmodel.Confirmers.ShowCardPrintsConfirmer.Confirm(CardSearchConfirmers.GetShowCardPrintsConfirmation(prints));
    }
  }
}
