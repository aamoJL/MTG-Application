using System.ComponentModel.DataAnnotations;

namespace MTGApplication.Models.DTOs;

/// <summary>
/// Base class for card data transfer objects
/// </summary>
public abstract class CardDTO
{
  public CardDTO() { }
  public CardDTO(string name, int count)
  {
    Name = name;
    Count = count;
  }

  [Key] public int Id { get; init; }
  public string Name { get; init; }
  public int Count { get; set; }
}
