using MTGApplication.Features.CardCollection.Services;
using MTGApplication.General.Models;
using MTGApplication.General.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace MTGApplication.Features.CardCollection.UseCases;

public partial class CardCollectionViewModelCommands
{
  public class ShowCardPrints(CardCollectionViewModel viewmodel) : ViewModelAsyncCommand<CardCollectionViewModel, CardCollectionMTGCard>(viewmodel)
  {
    protected override async Task Execute(CardCollectionMTGCard card)
    {
      var prints = (await Viewmodel.Worker.DoWork(Viewmodel.Importer.ImportFromUri(pageUri: card.Info.PrintSearchUri, paperOnly: true, fetchAll: true))).Found;

      await Viewmodel.Confirmers.ShowCardPrintsConfirmer.Confirm(CardCollectionConfirmers.GetShowCardPrintsConfirmation(prints.Select(x => new MTGCard(x.Info))));
    }
  }
}