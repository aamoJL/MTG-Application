using MTGApplication.Features.CardCollectionEditor.CardCollectionList.Services;
using MTGApplication.Features.CardCollectionEditor.CardCollectionList.ViewModels;
using MTGApplication.General.Services.IOServices;
using MTGApplication.General.Services.NotificationService.UseCases;
using MTGApplication.General.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MTGApplication.Features.CardCollectionEditor.CardCollectionList.UseCases;

public partial class CardCollectionEditorViewModelCommands
{
  public class ExportCards(CardCollectionListViewModel viewmodel) : ViewModelAsyncCommand<CardCollectionListViewModel>(viewmodel)
  {
    protected override bool CanExecute() => !string.IsNullOrEmpty(Viewmodel.Name);

    protected override async Task Execute()
    {
      if (!CanExecute()) return;

      if (await Viewmodel.Confirmers.ExportCardsConfirmer.Confirm(
        CardCollectionListConfirmers.GetExportCardsConfirmation(string.Join(Environment.NewLine, Viewmodel.CollectionList.Cards.Select(x => x.Info.ScryfallId))))
        is not string response || string.IsNullOrEmpty(response))
        return;

      Viewmodel.ClipboardService.CopyToClipboard(response);
      new SendNotification(Viewmodel.Notifier).Execute(ClipboardService.CopiedNotification);
    }
  }
}