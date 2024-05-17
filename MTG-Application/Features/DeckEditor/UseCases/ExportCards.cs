using MTGApplication.General.Models.Card;
using MTGApplication.General.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MTGApplication.Features.DeckEditor;

public class ExportCards : UseCase<ExportCards.Args, string>
{
  public record Args(IEnumerable<MTGCard> Cards, string ByProperty);

  public override string Execute(Args args)
  {
    var (cards, byProperty) = args;

    return byProperty switch
    {
      "Id" => string.Join(Environment.NewLine, cards.Select(x => x.Info.ScryfallId)),
      "Name" => string.Join(Environment.NewLine, cards.Select(x => x.Info.Name)),
      _ => null,
    };
  }
}