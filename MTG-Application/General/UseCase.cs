namespace MTGApplication.General;

public abstract class UseCase
{
  public abstract void Execute();
}

public abstract class UseCase<Response>
{
  public abstract Response Execute();
}