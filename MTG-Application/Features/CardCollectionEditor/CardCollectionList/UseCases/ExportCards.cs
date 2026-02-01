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
  public class ExportCards(CardCollectionListViewModel viewmodel) : AsyncCommand
  {
    protected override bool CanExecute() => !string.IsNullOrEmpty(viewmodel.Name);

    protected override async Task Execute()
    {
      if (!CanExecute()) return;

      if (await viewmodel.Confirmers.ExportCardsConfirmer.Confirm(
        CardCollectionListConfirmers.GetExportCardsConfirmation(string.Join(Environment.NewLine, viewmodel.Cards.Select(x => x.Info.ScryfallId))))
        is not string response || string.IsNullOrEmpty(response))
        return;

      try
      {
        viewmodel.ClipboardService.CopyToClipboard(response);
        new SendNotification(viewmodel.Notifier).Execute(ClipboardService.CopiedNotification);
      }
      catch { }
    }
  }
}