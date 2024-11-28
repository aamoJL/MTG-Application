using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;

namespace MTGApplication.General.ViewModels;

public abstract class ViewModelCommand<TViewModel>
{
  protected ViewModelCommand(TViewModel viewmodel)
  {
    Command = new(execute: Execute, canExecute: CanExecute);
    Viewmodel = viewmodel;
  }

  public RelayCommand Command { get; }

  protected TViewModel Viewmodel { get; }

  protected virtual bool CanExecute() => true;

  protected abstract void Execute();
}

public abstract class ViewModelCommand<TViewModel, TParam>
{
  protected ViewModelCommand(TViewModel viewmodel)
  {
    Command = new(execute: Execute, canExecute: CanExecute);
    Viewmodel = viewmodel;
  }

  public RelayCommand<TParam> Command { get; }

  protected TViewModel Viewmodel { get; }

  protected virtual bool CanExecute(TParam? param) => true;

  protected abstract void Execute(TParam? param);
}

public abstract class ViewModelAsyncCommand<TViewModel>
{
  protected ViewModelAsyncCommand(TViewModel viewmodel)
  {
    Command = new(execute: Execute, canExecute: CanExecute);
    Viewmodel = viewmodel;
  }

  public AsyncRelayCommand Command { get; }

  protected TViewModel Viewmodel { get; }

  protected virtual bool CanExecute() => true;

  protected abstract Task Execute();
}

public abstract class ViewModelAsyncCommand<TViewModel, TParam>
{
  protected ViewModelAsyncCommand(TViewModel viewmodel)
  {
    Command = new(execute: Execute, canExecute: CanExecute);
    Viewmodel = viewmodel;
  }

  public AsyncRelayCommand<TParam> Command { get; }

  protected TViewModel Viewmodel { get; }

  protected virtual bool CanExecute(TParam? param) => true;

  protected abstract Task Execute(TParam? param);
}