using MTGApplication.Features.DeckEditor.Services.Cardlist;
using MTGApplication.General.Models.Card;
using MTGApplication.General.Services.IOService;
using MTGApplication.General.Services.NotificationService.UseCases;
using MTGApplication.General.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTGApplication.Features.DeckEditor;

public partial class CardListViewModelCommands
{
  public class ExportCards(CardListViewModel viewmodel) : ViewModelAsyncCommand<CardListViewModel, string>(viewmodel)
  {
    protected override bool CanExecute(string byProperty) => byProperty is "Id" or "Name";

    protected override async Task Execute(string byProperty)
    {
      if (!CanExecute(byProperty)) return;

      if (await Viewmodel.Confirmers.ExportConfirmer.Confirm(CardListConfirmers.GetExportConfirmation(GetExportString(Viewmodel.Cards, byProperty)))
        is not string response || string.IsNullOrEmpty(response))
        return;

      Viewmodel.ClipboardService.CopyToClipboard(response);

      new SendNotification(Viewmodel.Notifier).Execute(ClipboardService.CopiedNotification);
    }

    private static string GetExportString(IEnumerable<DeckEditorMTGCard> cards, string byProperty)
    {
      return byProperty switch
      {
        "Id" => string.Join(Environment.NewLine, cards.Select(x => x.Info.ScryfallId)),
        "Name" => string.Join(Environment.NewLine, cards.Select(x => x.Info.Name)),
        _ => string.Empty,
      };
    }
  }
}