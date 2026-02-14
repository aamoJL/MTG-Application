using MTGApplication.General.Services.ConfirmationService;

namespace MTGApplicationTests.TestUtility.Services;

[Obsolete]
public class TestConfirmer<TReturn, TArgs> : Confirmer<TReturn, TArgs>
{
  public bool Confirmed { get; private set; } = false;

  public override Func<Confirmation<TArgs>, Task<TReturn?>>? OnConfirm
  {
    protected get { Confirmed = true; return base.OnConfirm; }
    set => base.OnConfirm = value;
  }
}

[Obsolete]
public class TestConfirmer<TReturn> : Confirmer<TReturn>
{
  public bool Confirmed { get; private set; } = false;

  public override Func<Confirmation, Task<TReturn?>>? OnConfirm
  {
    protected get { Confirmed = true; return base.OnConfirm; }
    set => base.OnConfirm = value;
  }
}

[Obsolete]
public static class ConfirmationAssert
{
  public static void ConfirmationShown<TReturn, TArgs>(TestConfirmer<TReturn, TArgs> confirmer)
    => Assert.IsTrue(confirmer.Confirmed);

  public static void ConfirmationShown<TReturn>(TestConfirmer<TReturn> confirmer)
    => Assert.IsTrue(confirmer.Confirmed);

  public static void ConfirmationNotShown<TReturn>(TestConfirmer<TReturn> confirmer)
    => Assert.IsFalse(confirmer.Confirmed);
}