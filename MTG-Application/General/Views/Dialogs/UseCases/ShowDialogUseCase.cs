using MTGApplication.General.ViewModels;
using System.Threading.Tasks;
using static MTGApplication.General.Services.ConfirmationService.DialogService;

namespace MTGApplication.General.Views.Dialogs;

public abstract class ShowDialogUseCase<TReturn>(DialogWrapper dialogWrapper) : UseCase<(string title, string message), Task<TReturn>>
{
  public DialogWrapper DialogWrapper { get; } = dialogWrapper;

  public override async Task<TReturn> Execute((string title, string message) args)
  {
    var (title, message) = args;

    return await ShowDialog(title, message);
  }

  protected abstract Task<TReturn> ShowDialog(string title, string message);
}

public abstract class ShowDialogUseCase<TReturn, TData>(DialogWrapper dialogWrapper) : UseCase<(string title, string message, TData data), Task<TReturn>>
{
  public DialogWrapper DialogWrapper { get; } = dialogWrapper;

  public override async Task<TReturn> Execute((string title, string message, TData data) args)
  {
    var (title, message, data) = args;

    return await ShowDialog(title, message, data);
  }

  protected abstract Task<TReturn> ShowDialog(string title, string message, TData data);
}