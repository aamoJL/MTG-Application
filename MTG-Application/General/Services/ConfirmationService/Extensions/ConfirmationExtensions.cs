namespace MTGApplication.General.Services.ConfirmationService.Extensions;

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
}
