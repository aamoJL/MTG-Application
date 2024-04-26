using MTGApplication.General.Databases.Repositories;
using MTGApplication.Models.DTOs;
using System.Threading.Tasks;
using static MTGApplication.General.Services.ConfirmationService.ConfirmationService;

namespace MTGApplication.Features.CardDeck;

public class SaveUnsavedChangesUseCase : SaveDeckUseCase
{
  public SaveUnsavedChangesUseCase(IRepository<MTGCardDeckDTO> repository) : base(repository) { }

  public Confirmer<ConfirmationResult> UnsavedChangesConfirmation { get; set; } = new();

  public override async Task<ConfirmationResult> Execute(Args args)
  {
    var deck = args.Deck;

    var saveUnsavedResult = await UnsavedChangesConfirmation.Confirm(new(
      Title: "Save unsaved changes?",
      Message: $"{(string.IsNullOrEmpty(deck.Name) ? "Unnamed deck" : $"'{deck.Name}'")} has unsaved changes. Would you like to save the deck?"));

    return saveUnsavedResult switch
    {
      ConfirmationResult.Yes => await base.Execute(args),
      ConfirmationResult.No => ConfirmationResult.No,
      _ => saveUnsavedResult,
    };
  }
}