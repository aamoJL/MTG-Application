using System.ComponentModel.DataAnnotations;

namespace MTGApplication.Models;

/// <summary>
/// Base class for card data transfer objects
/// </summary>
public abstract class CardDTO
{
  public CardDTO() { }

  [Key]
  public int Id { get; init; }
  public string Name { get; init; }
  public int Count { get; set; }
}
