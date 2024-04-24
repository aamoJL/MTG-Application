using MTGApplication.General.Databases.Repositories;
using MTGApplication.General.Databases.Repositories.MTGDeckRepository;
using MTGApplication.General.Services.ConfirmationService;
using MTGApplication.Models.DTOs;
using System.Threading.Tasks;
using static MTGApplication.General.Services.ConfirmationService.DialogService;

namespace MTGApplication.Features.CardDeck;

public class ProcessUnsavedChangesDialogUseCase : ShowDialogUseCase<string>
{
  public ProcessUnsavedChangesDialogUseCase(DialogWrapper wrapper, IRepository<MTGCardDeckDTO> repository, string currentName) : base(wrapper)
  {
    Wrapper = wrapper;
    Repository = repository;
    CurrentName = currentName;
  }

  public DialogWrapper Wrapper { get; }
  public IRepository<MTGCardDeckDTO> Repository { get; }
  public string CurrentName { get; }

  public override async Task Execute()
  {
    await new ShowUnsavedChangesDialogUseCase(Wrapper, CurrentName)
    {
      OnPrimary = async (confirmation) =>
      {
        if (confirmation is true)
        {
          await new ShowSaveDeckDialogUseCase(Wrapper, CurrentName)
          {
            OnPrimary = async (saveName) =>
            {
              if (!string.IsNullOrEmpty(saveName) && saveName != CurrentName && await new DeckExistsUseCase(Repository).Execute(saveName))
              {
                // Deck with the given name exists already
                await new ShowDeckOverrideDialogUseCase(DialogWrapper, saveName)
                {
                  OnPrimary = (_) => OnPrimary?.Invoke(saveName),
                  OnSecondary = (_) => OnSecondary?.Invoke(null),
                  OnCancel = (_) => OnCancel?.Invoke(null)
                }.Execute();
              }
              else OnPrimary?.Invoke(saveName);
            },
            OnSecondary = (_) => OnSecondary?.Invoke(null),
            OnCancel = (_) => OnCancel?.Invoke(null)
          }.Execute();
        }
      },
      OnSecondary = (_) => OnSecondary?.Invoke(null),
      OnCancel = (_) => OnCancel?.Invoke(null)
    }.Execute();
  }
}
