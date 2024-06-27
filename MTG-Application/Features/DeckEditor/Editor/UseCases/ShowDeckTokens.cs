using MTGApplication.Features.DeckEditor.Editor.Services;
using MTGApplication.Features.DeckEditor.ViewModels;
using MTGApplication.General.Models;
using MTGApplication.General.Services.Importers.CardImporter.UseCases;
using MTGApplication.General.ViewModels;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTGApplication.Features.DeckEditor.Editor.UseCases;

public partial class DeckEditorViewModelCommands
{
  public class ShowDeckTokens(DeckEditorViewModel viewmodel) : ViewModelAsyncCommand<DeckEditorViewModel>(viewmodel)
  {
    protected override bool CanExecute() => Viewmodel.Commander != null || Viewmodel.DeckCardList.Cards.Count != 0;

    protected override async Task Execute()
    {
      if (!CanExecute()) return;

      var stringBuilder = new StringBuilder();

      stringBuilder.AppendJoin(Environment.NewLine, Viewmodel.DeckCardList.Cards.Where(c => c.Info.Tokens.Length > 0).Select(
        c => string.Join(Environment.NewLine, c.Info.Tokens.Select(t => string.Join(Environment.NewLine, t.ScryfallId.ToString())))));

      if (Viewmodel.Commander != null)
        stringBuilder.AppendJoin(Environment.NewLine, Viewmodel.Commander.Info.Tokens.Select(t => t.ScryfallId.ToString()));

      if (Viewmodel.Partner != null)
        stringBuilder.AppendJoin(Environment.NewLine, Viewmodel.Partner.Info.Tokens.Select(t => t.ScryfallId.ToString()));

      var tokens = (await Viewmodel.Worker.DoWork(new FetchCardsWithImportString(Viewmodel.Importer).Execute(stringBuilder.ToString()))).Found
        .DistinctBy(t => t.Info.OracleId).Select(x => x.Info); // Filter duplicates out using OracleId

      await Viewmodel.Confirmers.ShowTokensConfirmer.Confirm(DeckEditorConfirmers.GetShowTokensConfirmation(tokens.Select(x => new MTGCard(x))));
    }
  }
}