using System;

namespace MTGApplication.Models.DTOs;

/// <summary>
/// Data transfer object for <see cref="MTGCard"/> class
/// </summary>
public class MTGCardDTO : CardDTO
{
  private MTGCardDTO() : base() { }
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
