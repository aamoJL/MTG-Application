using MTGApplication.Features.CardCollectionEditor.CardCollectionList.Models;
using MTGApplication.Features.CardCollectionEditor.CardCollectionList.Services;
using MTGApplication.Features.CardCollectionEditor.CardCollectionList.ViewModels;
using MTGApplication.General.Models;
using MTGApplication.General.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace MTGApplication.Features.CardCollectionEditor.CardCollectionList.UseCases;

public partial class CardCollectionEditorViewModelCommands
{
  public class ShowCardPrints(CardCollectionListViewModel viewmodel) : ViewModelAsyncCommand<CardCollectionListViewModel, CardCollectionMTGCard>(viewmodel)
  {
    protected override async Task Execute(CardCollectionMTGCard card)
    {
      var prints = (await Viewmodel.Worker.DoWork(Viewmodel.Importer.ImportFromUri(pageUri: card.Info.PrintSearchUri, paperOnly: true, fetchAll: true))).Found;

      await Viewmodel.Confirmers.ShowCardPrintsConfirmer.Confirm(CardCollectionListConfirmers.GetShowCardPrintsConfirmation(prints.Select(x => new MTGCard(x.Info))));
    }
  }
}