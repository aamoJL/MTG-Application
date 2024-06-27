using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.General.Services.ConfirmationService;

namespace MTGApplicationTests.TestUtility.Services;

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

public static class ConfirmationAssert
{
  public static async Task ConfirmationShown(Func<Task> task)
    => await Assert.ThrowsExceptionAsync<ConfirmationException>(task);
}