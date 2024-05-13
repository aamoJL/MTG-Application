namespace MTGApplication.General.Services.ConfirmationService;

public static partial class ConfirmationExtensions
{
  public static ConfirmationResult ToConfirmationResult(this bool? value)
  {
    return value switch
    {
      true => ConfirmationResult.Yes,
      false => ConfirmationResult.No,
      _ => ConfirmationResult.Cancel,
    };
  }

  public static ConfirmationResult FailureFromNull(object value)
    => value != null ? ConfirmationResult.Yes : ConfirmationResult.Failure;
}
