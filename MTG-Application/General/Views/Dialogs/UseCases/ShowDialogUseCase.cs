using Microsoft.UI.Xaml;
using MTGApplication.General.ViewModels;
using System.Threading.Tasks;

namespace MTGApplication.General.Views.Dialogs.UseCases;

public abstract class ShowDialogUseCase<TReturn>(XamlRoot root) : UseCase<(string title, string message), Task<TReturn>>
{
  public XamlRoot Root { get; } = root;

  public override async Task<TReturn> Execute((string title, string message) args)
  {
    var (title, message) = args;

    return await ShowDialog(title, message);
  }

  protected abstract Task<TReturn> ShowDialog(string title, string message);
}

public abstract class ShowDialogUseCase<TReturn, TData>(XamlRoot root) : UseCase<(string title, string message, TData data), Task<TReturn>>
{
  public XamlRoot Root { get; } = root;

  public override async Task<TReturn> Execute((string title, string message, TData data) args)
  {
    var (title, message, data) = args;

    return await ShowDialog(title, message, data);
  }

  protected abstract Task<TReturn> ShowDialog(string title, string message, TData data);
}