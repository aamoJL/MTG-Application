using MTGApplication.Features.DeckEditor.Editor.Services;
using MTGApplication.Features.DeckEditor.ViewModels;
using MTGApplication.General.Models;
using MTGApplication.General.Services.Importers.CardImporter.UseCases;
using MTGApplication.General.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTGApplication.Features.DeckEditor.Editor.UseCases;

public partial class DeckEditorViewModelCommands
{
  public class ShowDeckTokens(DeckEditorViewModel viewmodel) : AsyncCommand
  {
    public DeckEditorViewModel Viewmodel { get; } = viewmodel;

    protected override bool CanExecute() => Viewmodel.Commander.Card != null || Viewmodel.DeckCardList.Cards.Count != 0;

    protected override async Task Execute()
    {
      if (!CanExecute())
        return;

      var cards = new List<MTGCard?>(
        [.. Viewmodel.DeckCardList.Cards,
        Viewmodel.Commander.Card,
        Viewmodel.Partner.Card]).OfType<MTGCard>();

      try
      {
        var tokens = (await (Viewmodel as IWorker).DoWork(new FetchTokenCards(Viewmodel.Importer).Execute(cards))).Found
          .Select(x => new MTGCard(x.Info));

        await Viewmodel.Confirmers.ShowTokensConfirmer.Confirm(DeckEditorConfirmers.GetShowTokensConfirmation(tokens));
      }
      catch (Exception e)
      {
        Viewmodel.Notifier.Notify(new(General.Services.NotificationService.NotificationService.NotificationType.Error, $"Error: {e.Message}"));
      }
    }
  }
}