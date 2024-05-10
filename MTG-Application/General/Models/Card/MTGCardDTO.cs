using System;

namespace MTGApplication.General.Models.Card;

/// <summary>
/// Data transfer object for <see cref="MTGCard"/> class
/// </summary>
public class MTGCardDTO : CardDTO
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

  public MTGCardDTO Copy() => new(Name, ScryfallId, OracleId, SetCode, CollectorNumber, Count);

  public bool Compare(MTGCardDTO other, bool includeId = false, bool includeCount = false)
  {
    if(other == null) return false;

    if (includeId && Id != other.Id) return false;
    if (Name != other.Name) return false;
    if (includeCount && Count != other.Count) return false;
    if (ScryfallId != other.ScryfallId) return false;
    if (OracleId != other.OracleId) return false;
    if (SetCode != other.SetCode) return false;
    if (CollectorNumber != other.CollectorNumber) return false;

    return true;
  }
}