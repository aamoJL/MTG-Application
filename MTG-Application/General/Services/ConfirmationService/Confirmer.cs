using System;
using System.Threading.Tasks;

namespace MTGApplication.General.Services.ConfirmationService;

public class Confirmer<TReturn, TArgs>
{
  public virtual Func<Confirmation<TArgs>, Task<TReturn>> OnConfirm { protected get; set; }

  public async Task<TReturn> Confirm(Confirmation<TArgs> confirmation)
  {
    if (OnConfirm == null || confirmation == null) return default;

    var result = await OnConfirm.Invoke(confirmation);
    return result;
  }
}

public class Confirmer<TReturn>
{
  public virtual Func<Confirmation, Task<TReturn>> OnConfirm { protected get; set; }

  public async Task<TReturn> Confirm(Confirmation confirmation)
    => OnConfirm == null ? default : await OnConfirm.Invoke(confirmation);
}
