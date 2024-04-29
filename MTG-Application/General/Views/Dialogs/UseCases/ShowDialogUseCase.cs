using MTGApplication.General.UseCases;
using System.Threading.Tasks;
using static MTGApplication.General.Services.ConfirmationService.DialogService;

namespace MTGApplication.Views.Dialogs;

public abstract class ShowDialogUseCase<TReturn> : UseCase<(string title, string message), Task<TReturn>>
{
  protected ShowDialogUseCase(DialogWrapper dialogWrapper) => DialogWrapper = dialogWrapper;

  public DialogWrapper DialogWrapper { get; }

  public override async Task<TReturn> Execute((string title, string message) args)
  {
    var (title, message) = args;

    return await ShowDialog(title, message);
  }

  protected abstract Task<TReturn> ShowDialog(string title, string message);
}

public abstract class ShowDialogUseCase<TReturn, TData> : UseCase<(string title, string message, TData data), Task<TReturn>>
{
  protected ShowDialogUseCase(DialogWrapper dialogWrapper) => DialogWrapper = dialogWrapper;

  public DialogWrapper DialogWrapper { get; }

  public override async Task<TReturn> Execute((string title, string message, TData data) args)
  {
    var (title, message, data) = args;

    return await ShowDialog(title, message, data);
  }

  protected abstract Task<TReturn> ShowDialog(string title, string message, TData data);
}

// TODO: remove if not needed
//public class MTGDeckEditorDialogs
//{
//  public virtual GridViewDialog<MTGCardViewModel> GetCardPrintDialog(MTGCardViewModel[] printViewModels)
//    => new("Change card print", "MTGPrintGridViewItemTemplate", "MTGAdaptiveGridViewStyle") { Items = printViewModels, SecondaryButtonText = string.Empty };

//  public virtual GridViewDialog<MTGCardViewModel> GetTokenPrintDialog(MTGCardViewModel[] printViewModels)
//    => new("Tokens", "MTGPrintGridViewItemTemplate", "MTGAdaptiveGridViewStyle") { Items = printViewModels, SecondaryButtonText = string.Empty, PrimaryButtonText = string.Empty };
//}