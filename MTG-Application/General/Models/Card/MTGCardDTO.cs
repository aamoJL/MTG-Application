using System;

namespace MTGApplication.General.Models.Card;

/// <summary>
/// Data transfer object for <see cref="MTGCard"/> class
/// </summary>
public record MTGCardDTO : CardDTO
{
  private MTGCardDTO() : base() { }

  public MTGCardDTO(string name, Guid scryfallId, Guid oracleId, string setCode, string collectorNumber, int count) : base(name, count)
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

  public Guid ScryfallId { get; set; }
  public Guid OracleId { get; set; }
  public string SetCode { get; set; }
  public string CollectorNumber { get; set; }

  /// <summary>
  /// Compares DTOs, excluding Id and Count
  /// </summary>
  public bool Compare(MTGCardDTO other)
  {
    var x = this with { Id = 0, Count = 1};
    var y = other with { Id = 0, Count = 1 };

    return x.Equals(y);
  }
}