﻿using System;
using System.Threading.Tasks;

namespace MTGApplication.General.Services.ConfirmationService;

public static partial class ConfirmationService
{
  public enum ConfirmationResult
  {
    Yes, No, Failure, Cancel
  }

  public record Confirmation<TArgs>(string Title, string Message, TArgs Data);
  public record Confirmation(string Title, string Message);

  public class Confirmer<TReturn, TArgs>
  {
    public Func<Confirmation<TArgs>, Task<TReturn>> OnConfirm { private get; set; }

    public async Task<TReturn> Confirm(Confirmation<TArgs> confirmation)
      => OnConfirm == null ? default : await OnConfirm.Invoke(confirmation);
  }

  public class Confirmer<TReturn>
  {
    public Func<Confirmation, Task<TReturn>> OnConfirm { private get; set; }

    public async Task<TReturn> Confirm(Confirmation confirmation)
      => OnConfirm == null ? default : await OnConfirm.Invoke(confirmation);
  }
}

public static partial class ConfirmationService
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
