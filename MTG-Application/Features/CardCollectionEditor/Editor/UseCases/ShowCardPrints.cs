using MTGApplication.Features.CardCollection.Editor.ViewModels;
using MTGApplication.Features.CardCollectionEditor.CardCollection.Services;
using MTGApplication.Features.CardCollectionEditor.CardCollectionList.Models;
using MTGApplication.General.Models;
using MTGApplication.General.Services.NotificationService.UseCases;
using MTGApplication.General.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MTGApplication.Features.CardCollection.Editor.UseCases;

public partial class CardCollectionEditorViewModelCommands
{
  public class ShowCardPrints(CardCollectionEditorViewModel viewmodel) : AsyncCommand<CardCollectionMTGCard>
  {
    protected override async Task Execute(CardCollectionMTGCard? card)
    {
      try
      {
        ArgumentNullException.ThrowIfNull(card);

        var prints = (await viewmodel.Worker.DoWork(viewmodel.Importer.ImportWithUri(pageUri: card.Info.PrintSearchUri, paperOnly: true, fetchAll: true))).Found;

        await viewmodel.Confirmers.CardCollectionConfirmers.ShowCardPrintsConfirmer.Confirm(CardCollectionConfirmers.GetShowCardPrintsConfirmation(prints.Select(x => new MTGCard(x.Info))));
      }
      catch (Exception e)
      {
        new SendNotification(viewmodel.Notifier).Execute(new(General.Services.NotificationService.NotificationService.NotificationType.Error, $"Error: {e.Message}"));
      }
    }
  }
}