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
    public virtual Func<Confirmation<TArgs>, Task<TReturn>> OnConfirm { protected get; set; }

    public async Task<TReturn> Confirm(Confirmation<TArgs> confirmation)
      => OnConfirm == null ? default : await OnConfirm.Invoke(confirmation);
  }

  public class Confirmer<TReturn>
  {
    public virtual Func<Confirmation, Task<TReturn>> OnConfirm { protected get; set; }

    public async Task<TReturn> Confirm(Confirmation confirmation)
      => OnConfirm == null ? default : await OnConfirm.Invoke(confirmation);
  }
}