using System.Threading.Tasks;

namespace MTGApplication.General.ViewModels;

public abstract class UseCaseAction
{
  public abstract void Execute();
}

public abstract class UseCaseAction<TArg>
{
  public abstract void Execute(TArg arg);
}

public abstract class UseCase<TResponse>
{
  public abstract TResponse Execute();
}

public abstract class UseCaseFunc<TArg, TReturn>
{
  public abstract TReturn Execute(TArg arg);
}

public abstract class AsyncUseCase<TArg, TReturn>
{
  public abstract Task<TReturn> ExecuteAsync(TArg arg);
}