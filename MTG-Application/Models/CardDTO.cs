using System.ComponentModel.DataAnnotations;

namespace MTGApplication.Models
{
  public class CardDTO
  {
    public CardDTO() { }

    [Key]
    public int Id { get; init; }
    public string Name { get; init; }
    public int Count { get; set; }
  }
}
