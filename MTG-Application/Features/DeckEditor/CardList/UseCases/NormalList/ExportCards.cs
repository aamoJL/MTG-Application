using MTGApplication.Features.DeckEditor.CardList.Services;
using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplication.General.Services.Exporters;
using MTGApplication.General.Services.NotificationService.UseCases;
using MTGApplication.General.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplication.Features.DeckEditor.CardList.UseCases;

public partial class CardListViewModelCommands
{
  public class ExportCards(IList<DeckEditorMTGCard> cards, CardListConfirmers confirmers, Notifier notifier, IExporter<string> exporter) : AsyncCommand<string>
  {
    public IList<DeckEditorMTGCard> Cards { get; } = cards;
    private CardListConfirmers Confirmers { get; } = confirmers;
    private Notifier Notifier { get; } = notifier;
    private IExporter<string> Exporter { get; } = exporter;

    protected override bool CanExecute(string? byProperty) => byProperty is "Id" or "Name";

    protected override async Task Execute(string? byProperty)
    {
      if (!CanExecute(byProperty))
        return;

      if (await Confirmers.ExportConfirmer.Confirm(CardListConfirmers.GetExportConfirmation(GetExportString(Cards, byProperty!)))
        is not string response || string.IsNullOrEmpty(response))
        return;

      await Exporter.Export(response);

      new ShowNotification(Notifier).Execute(Exporter.SuccessNotification);
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