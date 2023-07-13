using System;

namespace MTGApplication.Models.DTOs;

/// <summary>
/// Data transfer object for <see cref="MTGCard"/> class
/// </summary>
public class MTGCardDTO : CardDTO
{
  private MTGCardDTO() : base() { }
  public MTGCardDTO(MTGCard card) : base(card.Info.Name, card.Count) => ScryfallId = card.Info.ScryfallId;

  public Guid ScryfallId { get; init; }
}
