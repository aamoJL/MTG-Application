namespace MTGApplication.Features.MTGCardSearch.UseCases;

public abstract class UseCase<Args, Response>
{
  public abstract Response Execute(Args args);
}