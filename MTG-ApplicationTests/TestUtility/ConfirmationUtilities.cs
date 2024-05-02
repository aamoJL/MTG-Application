using Microsoft.VisualStudio.TestTools.UnitTesting;
using static MTGApplication.General.Services.ConfirmationService.ConfirmationService;

namespace MTGApplicationTests.TestUtility;

public class ConfirmationException : UnitTestAssertException { }

public class TestExceptionConfirmer<TReturn, TArgs> : Confirmer<TReturn, TArgs>
{
  public override Func<Confirmation<TArgs>, Task<TReturn>> OnConfirm
  {
    protected get => (arg) => { throw new ConfirmationException(); };
    set => base.OnConfirm = value;
  }
}

public class TestExceptionConfirmer<TReturn> : Confirmer<TReturn>
{
  public override Func<Confirmation, Task<TReturn>> OnConfirm
  {
    protected get => (arg) => { throw new ConfirmationException(); };
    set => base.OnConfirm = value;
  }
}