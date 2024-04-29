using MTGApplication.General.Databases.Repositories;
using MTGApplication.General.Models.CardDeck;
using System.Threading.Tasks;
using static MTGApplication.General.Services.ConfirmationService.ConfirmationService;

namespace MTGApplication.Features.CardDeck;

public class SaveUnsavedChanges : SaveDeck
{
  public SaveUnsavedChanges(IRepository<MTGCardDeckDTO> repository) : base(repository) { }

  public Confirmer<ConfirmationResult> UnsavedChangesConfirmation { get; set; } = new();

  public override async Task<ConfirmationResult> Execute(MTGCardDeck deck)
  {
    var saveUnsavedResult = await UnsavedChangesConfirmation.Confirm(new(
      Title: "Save unsaved changes?",
      Message: $"{(string.IsNullOrEmpty(deck.Name) ? "Unnamed deck" : $"'{deck.Name}'")} has unsaved changes. Would you like to save the deck?"));

    return saveUnsavedResult switch
    {
      ConfirmationResult.Yes => await base.Execute(deck),
      ConfirmationResult.No => ConfirmationResult.No,
      _ => saveUnsavedResult,
    };
  }
}