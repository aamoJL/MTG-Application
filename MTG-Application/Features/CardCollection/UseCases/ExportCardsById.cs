using MTGApplication.General.Models.Card;
using MTGApplication.General.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MTGApplication.Features.CardCollection;

public class ExportCardsById : UseCase<IEnumerable<MTGCard>, string>
{
  public override string Execute(IEnumerable<MTGCard> cards)
    => string.Join(Environment.NewLine, cards.Select(x => x.Info.ScryfallId));
}