using MTGApplication.General.Models.Card;
using MTGApplication.General.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MTGApplication.Features.CardCollection.UseCases;

public class ExportCardsById : UseCase<IEnumerable<DeckEditorMTGCard>, string>
{
  public override string Execute(IEnumerable<DeckEditorMTGCard> cards)
    => string.Join(Environment.NewLine, cards.Select(x => x.Info.ScryfallId));
}