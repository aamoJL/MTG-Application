using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;

namespace MTGApplication.General.ViewModels;

public abstract class ViewModelCommand<TViewModel>(TViewModel viewmodel)
{
  public RelayCommand Command => new(execute: Execute, canExecute: CanExecute);

  protected TViewModel Viewmodel { get; } = viewmodel;

  protected virtual bool CanExecute() => true;

  protected abstract void Execute();
}

public abstract class ViewModelCommand<TViewModel, TParam>(TViewModel viewmodel)
{
  public RelayCommand<TParam> Command => new(execute: Execute, canExecute: CanExecute);

  protected TViewModel Viewmodel { get; } = viewmodel;

  protected virtual bool CanExecute(TParam param) => true;

  protected abstract void Execute(TParam param);
}

public abstract class ViewModelAsyncCommand<TViewModel>(TViewModel viewmodel)
{
  public AsyncRelayCommand Command => new(execute: Execute, canExecute: CanExecute);

  protected TViewModel Viewmodel { get; } = viewmodel;

  protected virtual bool CanExecute() => true;

  protected abstract Task Execute();
}

public abstract class ViewModelAsyncCommand<TViewModel, TParam>(TViewModel viewmodel)
{
  public AsyncRelayCommand<TParam> Command => new(execute: Execute, canExecute: CanExecute);

  protected TViewModel Viewmodel { get; } = viewmodel;

  protected virtual bool CanExecute(TParam param) => true;

  protected abstract Task Execute(TParam param);
}