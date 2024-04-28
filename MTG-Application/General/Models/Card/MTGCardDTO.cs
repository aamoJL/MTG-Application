using System;

namespace MTGApplication.General.Models.Card;

/// <summary>
/// Data transfer object for <see cref="MTGCard"/> class
/// </summary>
public class MTGCardDTO : CardDTO
{
  private MTGCardDTO() : base() { }

  public MTGCardDTO(Guid scryfallId, Guid oracleId, string setCode, string collectorNumber)
  {
    ScryfallId = scryfallId;
    OracleId = oracleId;
    SetCode = setCode;
    CollectorNumber = collectorNumber;
  }

  public MTGCardDTO(MTGCard card) : base(card.Info.Name, card.Count)
  {
    ScryfallId = card.Info.ScryfallId;
    OracleId = card.Info.OracleId;
    SetCode = card.Info.SetCode;
    CollectorNumber = card.Info.CollectorNumber;
  }

  public Guid ScryfallId { get; init; }
  public Guid OracleId { get; init; }
  public string SetCode { get; init; }
  public string CollectorNumber { get; init; }
}
