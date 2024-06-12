using MTGApplication.Features.DeckEditor.Editor.Services;
using MTGApplication.General.Models;
using MTGApplication.General.ViewModels;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTGApplication.Features.DeckEditor;

public partial class DeckEditorViewModelCommands
{
  public class ShowDeckTokens(DeckEditorViewModel viewmodel) : ViewModelAsyncCommand<DeckEditorViewModel>(viewmodel)
  {
    protected override bool CanExecute() => Viewmodel.Deck.Commander != null || Viewmodel.Deck.DeckCards.Count != 0;

    protected override async Task Execute()
    {
      var stringBuilder = new StringBuilder();

      stringBuilder.AppendJoin(Environment.NewLine, Viewmodel.DeckCardList.Cards.Where(c => c.Info.Tokens.Length > 0).Select(
        c => string.Join(Environment.NewLine, c.Info.Tokens.Select(t => string.Join(Environment.NewLine, t.ScryfallId.ToString())))));

      if (Viewmodel.Deck.Commander != null)
        stringBuilder.AppendJoin(Environment.NewLine, Viewmodel.Deck.Commander.Info.Tokens.Select(t => t.ScryfallId.ToString()));

      if (Viewmodel.Deck.CommanderPartner != null)
        stringBuilder.AppendJoin(Environment.NewLine, Viewmodel.Deck.CommanderPartner.Info.Tokens.Select(t => t.ScryfallId.ToString()));

      var tokens = (await Viewmodel.Worker.DoWork(Viewmodel.Importer.ImportFromString(stringBuilder.ToString()))).Found
        .DistinctBy(t => t.Info.OracleId).Select(x => x.Info); // Filter duplicates out using OracleId

      await Viewmodel.Confirmers.ShowTokensConfirmer.Confirm(DeckEditorConfirmers.GetShowTokensConfirmation(tokens.Select(x => new MTGCard(x))));
    }
  }
}