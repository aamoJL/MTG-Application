using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;

namespace MTGApplication.General.ViewModels;

public abstract class SyncCommand
{
  protected SyncCommand() => Command = new(execute: Execute, canExecute: CanExecute);

  public RelayCommand Command { get; }

  protected virtual bool CanExecute() => true;

  protected abstract void Execute();
}

public abstract class AsyncCommand
{
  protected AsyncCommand() => Command = new(execute: Execute, canExecute: CanExecute);

  public AsyncRelayCommand Command { get; }

  protected virtual bool CanExecute() => true;

  protected abstract Task Execute();
}

public abstract class SyncCommand<TParam>
{
  protected SyncCommand() => Command = new(execute: Execute, canExecute: CanExecute);

  public RelayCommand<TParam> Command { get; }

  protected virtual bool CanExecute(TParam? param) => true;

  protected abstract void Execute(TParam? param);
}

public abstract class AsyncCommand<TParam>
{
  protected AsyncCommand() => Command = new(execute: Execute, canExecute: CanExecute);

  public AsyncRelayCommand<TParam> Command { get; }

  protected virtual bool CanExecute(TParam? param) => true;

  protected abstract Task Execute(TParam? param);
}