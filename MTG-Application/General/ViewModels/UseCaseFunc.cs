namespace MTGApplication.General.ViewModels;

public abstract class UseCaseAction
{
  public abstract void Execute();
}

public abstract class UseCaseAction<TArg>
{
  public abstract void Execute(TArg arg);
}

public abstract class UseCaseFunc<TResponse>
{
  public abstract TResponse Execute();
}

public abstract class UseCaseFunc<TArg, TReturn>
{
  public abstract TReturn Execute(TArg arg);
}

public abstract class UseCaseFunc<TArg, TArg2, TReturn>
{
  public abstract TReturn Execute(TArg arg, TArg2 arg2);
}

public abstract class UseCaseFunc<TArg, TArg2, TArg3, TReturn>
{
  public abstract TReturn Execute(TArg arg, TArg2 arg2, TArg3 arg3);
}