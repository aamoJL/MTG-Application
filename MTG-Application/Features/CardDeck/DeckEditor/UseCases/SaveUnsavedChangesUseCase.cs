using MTGApplication.General.Databases.Repositories;
using MTGApplication.General.Services.ConfirmationService;
using MTGApplication.Models.DTOs;
using System.Threading.Tasks;

namespace MTGApplication.Features.CardDeck;

public class SaveUnsavedChangesUseCase : SaveDeckUseCase
{
  public SaveUnsavedChangesUseCase(IRepository<MTGCardDeckDTO> repository) : base(repository) { }

  public Confirmation<ConfirmationResult> UnsavedChangesConfirmation { get; set; } = new();

  public override async Task<ConfirmationResult> Execute(Args args)
  {
    var deck = args.Deck;

    var saveUnsavedResult = await UnsavedChangesConfirmation.Confirm(
      title: "Save unsaved changes?",
      message: $"{(string.IsNullOrEmpty(deck.Name) ? "Unnamed deck" : $"'{deck.Name}'")} has unsaved changes. Would you like to save the deck?");

    return saveUnsavedResult switch
    {
      ConfirmationResult.Success => await base.Execute(args),
      ConfirmationResult.Failure => ConfirmationResult.Failure,
      _ => ConfirmationResult.Cancel,
    };
  }
}