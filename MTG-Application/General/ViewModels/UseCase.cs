namespace MTGApplication.General.ViewModels;

public abstract class UseCase
{
  public abstract void Execute();
}

public abstract class UseCase<TResponse>
{
  public abstract TResponse Execute();
}

public abstract class UseCase<TArg, TReturn>
{
  public abstract TReturn Execute(TArg arg);
}