using MTGApplication.General.Services.ConfirmationService;
using System.Collections.Generic;

namespace MTGApplication.Features.CardCollection;

public class CardCollectionConfirmers
{
  public Confirmer<string, IEnumerable<string>> LoadCollectionConfirmer { get; init; } = new();

  public static Confirmation<IEnumerable<string>> GetLoadCollectionConfirmation(IEnumerable<string> data)
  {
    return new(
      Title: "Open collection",
      Message: "Name",
      Data: data);
  }
}