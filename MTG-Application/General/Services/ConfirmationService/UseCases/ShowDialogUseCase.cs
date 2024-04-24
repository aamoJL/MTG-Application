using Microsoft.UI.Xaml.Controls;
using MTGApplication.General.UseCases;
using System;
using System.Threading.Tasks;
using static MTGApplication.General.Services.ConfirmationService.DialogService;

namespace MTGApplication.General.Services.ConfirmationService;

public abstract class ShowDialogUseCase<T> : UseCase<Task>
{
  public ShowDialogUseCase(DialogWrapper wrapper) => DialogWrapper = wrapper;

  protected Dialog<T> Dialog { get; init; }
  protected DialogWrapper DialogWrapper { get; }

  public Action<T> OnPrimary { get; init; }
  public Action<T> OnSecondary { get; init; }
  public Action<T> OnCancel { get; init; }

  public async override Task Execute()
  {
    var dialogResult = await DialogWrapper.ShowAsync(Dialog);
    var inputResult = Dialog.ProcessResult(dialogResult);

    switch (dialogResult)
    {
      case ContentDialogResult.Primary: OnPrimary?.Invoke(inputResult); break;
      case ContentDialogResult.Secondary: OnSecondary?.Invoke(inputResult); break;
      default: OnCancel?.Invoke(inputResult); break;
    }
  }
}