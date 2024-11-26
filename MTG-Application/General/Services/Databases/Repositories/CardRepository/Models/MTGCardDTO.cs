using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplication.General.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace MTGApplication.General.Services.Databases.Repositories.CardRepository.Models;

/// <summary>
/// Data transfer object for <see cref="DeckEditorMTGCard"/> class
/// </summary>
public record MTGCardDTO
{
  private MTGCardDTO() { }
  public MTGCardDTO(string name, int count, Guid scryfallId, Guid oracleId, string setCode, string collectorNumber, string group)
  {
    Name = name;
    Count = count;
    ScryfallId = scryfallId;
    OracleId = oracleId;
    SetCode = setCode;
    CollectorNumber = collectorNumber;
    Group = group;
  }
  public MTGCardDTO(MTGCardInfo info, int count = 1)
  {
    Name = info.Name;
    Count = count;
    ScryfallId = info.ScryfallId;
    OracleId = info.OracleId;
    SetCode = info.SetCode;
    CollectorNumber = info.CollectorNumber;
  }

  [Key] public int Id { get; init; }
  [Required] public string Name { get; init; } = string.Empty;
  [Required] public int Count { get; set; } = 1;
  public Guid ScryfallId { get; set; } = Guid.Empty;
  public Guid OracleId { get; set; } = Guid.Empty;
  public string SetCode { get; set; } = string.Empty;
  public string CollectorNumber { get; set; } = string.Empty;
  [Required] public string Group { get; set; } = string.Empty;

  /// <summary>
  /// Compares DTOs, excluding Id and Count
  /// </summary>
  public bool Compare(MTGCardDTO other)
  {
    var x = this with { Id = 0, Count = 1 };
    var y = other with { Id = 0, Count = 1 };

    return x.Equals(y);
  }
}