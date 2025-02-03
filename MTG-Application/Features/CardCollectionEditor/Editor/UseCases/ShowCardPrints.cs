using MTGApplication.Features.CardCollection.Editor.ViewModels;
using MTGApplication.Features.CardCollectionEditor.CardCollection.Services;
using MTGApplication.Features.CardCollectionEditor.CardCollectionList.Models;
using MTGApplication.General.Models;
using MTGApplication.General.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MTGApplication.Features.CardCollection.Editor.UseCases;

public partial class CardCollectionEditorViewModelCommands
{
  public class ShowCardPrints(CardCollectionEditorViewModel viewmodel) : ViewModelAsyncCommand<CardCollectionEditorViewModel, CardCollectionMTGCard>(viewmodel)
  {
    protected override async Task Execute(CardCollectionMTGCard? card)
    {
      try
      {
        ArgumentNullException.ThrowIfNull(card);

        var prints = (await (Viewmodel as IWorker).DoWork(Viewmodel.Importer.ImportWithUri(pageUri: card.Info.PrintSearchUri, paperOnly: true, fetchAll: true))).Found;

        await Viewmodel.Confirmers.CardCollectionConfirmers.ShowCardPrintsConfirmer.Confirm(CardCollectionConfirmers.GetShowCardPrintsConfirmation(prints.Select(x => new MTGCard(x.Info))));
      }
      catch (Exception e)
      {
        Viewmodel.Notifier.Notify(new(General.Services.NotificationService.NotificationService.NotificationType.Error, $"Error: {e.Message}"));
      }
    }
  }
}