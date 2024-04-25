using System;
using System.Threading.Tasks;

namespace MTGApplication.General.Services.ConfirmationService;

public class Confirmation<TReturn, TArgs>
{
  public record ConfirmationData(string Title, string Message, TArgs Data);

  public Func<ConfirmationData, Task<TReturn>> OnConfirm { private get; set; }

  public async Task<TReturn> Confirm(string title, string message, TArgs data)
    => OnConfirm == null ? default : await OnConfirm.Invoke(new(title, message, data));
}

public class Confirmation<TReturn>
{
  public record ConfirmationData(string Title, string Message);

  public Func<ConfirmationData, Task<TReturn>> OnConfirm { private get; set; }

  public async Task<TReturn> Confirm(string title, string message)
    => OnConfirm == null ? default : await OnConfirm.Invoke(new(title, message));
}

public enum ConfirmationResult 
{ 
  Success, Failure, Cancel
}

public static class Confirmation
{
  public static ConfirmationResult ToConfirmationResult(this bool? value)
  {
    return value switch
    {
      true => ConfirmationResult.Success,
      false => ConfirmationResult.Failure,
      _ => ConfirmationResult.Cancel,
    };
  }

  public static ConfirmationResult FailureFromNull(object value) 
    => value != null ? ConfirmationResult.Success : ConfirmationResult.Failure;
}
