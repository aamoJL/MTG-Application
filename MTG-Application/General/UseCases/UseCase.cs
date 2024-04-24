namespace MTGApplication.General.UseCases;

public abstract class UseCase
{
  public abstract void Execute();
}

public abstract class UseCase<Response>
{
  public abstract Response Execute();
}

public abstract class UseCase<TArg, TReturn>
{
  public abstract TReturn Execute(TArg arg);
}