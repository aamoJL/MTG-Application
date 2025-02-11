using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.General.Services.ConfirmationService;

namespace MTGApplicationTests.TestUtility.Services;

public class TestConfirmer<TReturn, TArgs> : Confirmer<TReturn, TArgs>
{
  public bool Confirmed { get; private set; } = false;

  public override Func<Confirmation<TArgs>, Task<TReturn>> OnConfirm
  {
    protected get { Confirmed = true; return base.OnConfirm; }
    set => base.OnConfirm = value;
  }
}

public class TestConfirmer<TReturn> : Confirmer<TReturn>
{
  public bool Confirmed { get; private set; } = false;

  public override Func<Confirmation, Task<TReturn>> OnConfirm
  {
    protected get { Confirmed = true; return base.OnConfirm; }
    set => base.OnConfirm = value;
  }
}

public class TestDataOnlyConfirmer<TArgs> : DataOnlyConfirmer<TArgs>
{
  public bool Confirmed { get; private set; } = false;

  public override Func<Confirmation<TArgs>, Task> OnConfirm
  {
    protected get { Confirmed = true; return base.OnConfirm; }
    set => base.OnConfirm = value;
  }
}

public static class ConfirmationAssert
{
  public static void ConfirmationShown<TReturn, TArgs>(TestConfirmer<TReturn, TArgs> confirmer)
    => Assert.IsTrue(confirmer.Confirmed);

  public static void ConfirmationShown<TReturn>(TestConfirmer<TReturn> confirmer)
    => Assert.IsTrue(confirmer.Confirmed);

  public static void ConfirmationShown<TArgs>(TestDataOnlyConfirmer<TArgs> confirmer)
    => Assert.IsTrue(confirmer.Confirmed);

  public static void ConfirmationNotShown<TReturn, TArgs>(TestConfirmer<TReturn, TArgs> confirmer)
    => Assert.IsFalse(confirmer.Confirmed);

  public static void ConfirmationNotShown<TReturn>(TestConfirmer<TReturn> confirmer)
    => Assert.IsFalse(confirmer.Confirmed);
}